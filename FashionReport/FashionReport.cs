using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Lumina.Excel.Sheets;
using Dalamud.Interface;
using System.Reflection;
using System.IO;
using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Threading.Tasks;


namespace FashionReportCalculator;

public sealed class FashionReport : IDalamudPlugin
{
    public readonly WindowSystem WindowSystem = new("Fashion Report");
    internal static DisplayWindow DisplayWindow { get; set; } = new();
    internal static AboutWindow AboutWindow { get; set; } = new();

    public FashionReport(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<SERVICES>();
        GoogleSheetData.Initialize();
        SERVICES.AllItems = SERVICES.Data.GetExcelSheet<Item>()?.ToList() ?? new List<Item>();
        SERVICES.AllEquipItems = SERVICES.Data.GetExcelSheet<Item>()?.Where(item => item.ItemSortCategory.RowId != 0 && item.EquipSlotCategory.Value.RowId != 0).ToList() ?? null!;
        SERVICES.AllStains = SERVICES.Data.GetExcelSheet<Stain>().ToList();
        SERVICES.AllWeeklyFashionThemes = SERVICES.Data.GetExcelSheet<FashionCheckWeeklyTheme>()?.ToList() ?? new List<FashionCheckWeeklyTheme>();
        SERVICES.AllFashionThemeCategories = SERVICES.Data.GetExcelSheet<FashionCheckThemeCategory>()?.ToList() ?? new List<FashionCheckThemeCategory>();
        SERVICES.StainTable = SERVICES.AllStains.ToDictionary(stain => stain.RowId, stain =>
        {
            Item item = SERVICES.AllItems!.FirstOrDefault(x => x.Name.ToString() == stain.Name.ToString().TrimEnd() + " Dye");
            uint itemId = 0;
            uint iconId = 0;
            if (item.RowId != 0)
            {
                try { itemId = Convert.ToUInt32(item.GetType().GetProperty("RowId")?.GetValue(item)); } catch { itemId = 0; }
                try { iconId = Convert.ToUInt32(item.GetType().GetProperty("Icon")?.GetValue(item) ?? item.GetType().GetProperty("IconId")?.GetValue(item)); } catch { iconId = 0; }
            }
            return (stain.Name.ToString(), itemId, iconId);
        });
        SERVICES.frdata = FashionReportDataStorage.Load();
        EquippedGearService.Initialize();
        SERVICES.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "FashionCheck", OnFashionCheck);
        WindowSystem.AddWindow(DisplayWindow);
        WindowSystem.AddWindow(AboutWindow);
        SERVICES.CommandManager.AddHandler("/fr", new CommandInfo(OnFashionReport) { HelpMessage = "Open Fashion Report table for testing!" });
        SERVICES.CommandManager.AddHandler("/fashionreport", new CommandInfo(OnFashionReport) { HelpMessage = "Fashion Report calculator!" });
        SERVICES.Interface.UiBuilder.Draw += FashionReportDrawUI;
        SERVICES.Interface.UiBuilder.OpenMainUi += FashionReportUI;
        SERVICES.Interface.UiBuilder.OpenConfigUi += FashionConfigUI;
        FashionReportPoller.Initialize();
    }

    public void Dispose()
    {
        SERVICES.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "FashionCheck", OnFashionCheck);
        SERVICES.CommandManager.RemoveHandler("/fr");
        SERVICES.CommandManager.RemoveHandler("/fashionreport");
        SERVICES.Interface.UiBuilder.Draw -= FashionReportDrawUI;
        SERVICES.Interface.UiBuilder.OpenMainUi -= FashionReportUI;
        SERVICES.Interface.UiBuilder.OpenConfigUi -= FashionConfigUI;
        FashionReportPoller.Dispose();
        EquippedGearService.Close();
        WindowSystem.RemoveAllWindows();
        DisplayWindow.Dispose();
        AboutWindow.Dispose();
    }

    private static void OnFashionReport(string command, string args) => DisplayWindow.Toggle();
    private void FashionReportDrawUI() => WindowSystem.Draw();
    private void FashionReportUI() => DisplayWindow.Toggle();
    private void FashionConfigUI() => AboutWindow.Toggle();

    internal static async Task<nint> GetAddonSafe(string addonName, int timeoutMs = 5000, int pollIntervalMs = 100)
    {
        DateTime end = DateTime.Now.AddMilliseconds(timeoutMs);
        nint ptr;
        do
        {
            ptr = SERVICES.GameGui.GetAddonByName(addonName);
            unsafe
            {
                if (ptr != nint.Zero || ((AtkUnitBase*)ptr)->IsReady)
                    return ptr;
            }
            await Task.Delay(pollIntervalMs);
        } while (DateTime.Now < end);
        return nint.Zero;
    }

    private static async void OnFashionCheck(AddonEvent type, AddonArgs args)
    {
        try
        {
            LOG.Debug("OnFashionCheck triggered.");
            FashionCheck fashionCheck = new(await GetAddonSafe("FashionCheck"));
            if (fashionCheck.IsNull || !fashionCheck.IsReady)
            {
                fashionCheck = new(await GetAddonSafe("FashionCheck"));
                if (fashionCheck.IsNull) { LOG.Warning("FashionCheck addon failure."); return; }
            }
            while (string.IsNullOrEmpty(fashionCheck.TryGetAtkValue<string>(130)))
                await Task.Delay(100);
            uint oldWeek = SERVICES.frdata.Week;
            FashionReportDataStorage temp = new();
            LOG.Debug("Reading FashionCheck addon data...");
            temp.Week = FashionReportPoller.CurrentWeek;
            temp.WeeklyThemeName = fashionCheck.WeeklyTheme;
            temp.WeaponThemeName = fashionCheck.WeaponTheme;
            temp.HeadThemeName = fashionCheck.HeadTheme;
            temp.BodyThemeName = fashionCheck.BodyTheme;
            temp.GlovesThemeName = fashionCheck.HandsTheme;
            temp.LegsThemeName = fashionCheck.LegsTheme;
            temp.BootsThemeName = fashionCheck.FeetTheme;
            temp.EarringsThemeName = fashionCheck.EarringsTheme;
            temp.NecklaceThemeName = fashionCheck.NeckTheme;
            temp.BraceletThemeName = fashionCheck.WristTheme;
            temp.RightRingThemeName = fashionCheck.RightRingTheme;
            temp.LeftRingThemeName = fashionCheck.LeftRingTheme;
            await temp.ProcessorFromString();
            if (SERVICES.frdata != temp)
                SERVICES.frdata = temp;
            if ((SERVICES.frdata.Week != oldWeek || !await GoogleSheetData.IsWeekUpdated(SERVICES.frdata.Week)) && !FashionReportPoller.IsTuesdayComplete)
                _ = Task.Run(async () =>
                {
                    LOG.Debug("Saving to database ...");
                    await temp.SaveToDatabase();
                    LOG.Info($"FashionCheck week {SERVICES.frdata.Week} updated.");
                });
            if (FashionReportPoller.IsFridayComplete) return;
            if (temp.Weapon == null) return;
            nint g = await GetAddonSafe("FashionCheckScoreGauge", 10000, 100);
            FashionCheckScoreGauge Gauge = new(g);
            DyeStruct dyeEntry = new()
            {
                Score = Gauge.Score,
                WeaponItemId = fashionCheck.WeaponItemId,
                WeaponGlamourId = fashionCheck.WeaponGlamourId,
                WeaponDye1 = fashionCheck.WeaponShade1 != 0 ? fashionCheck.WeaponShade1 : null,
                WeaponDye2 = fashionCheck.WeaponShade2 != 0 ? fashionCheck.WeaponShade2 : null,
                WeaponTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.WeaponThemeName).RowId,
                WeaponPicture = fashionCheck.WeaponPicture,
                WeaponPictureInfo = fashionCheck.WeaponPictureInfo,
                HeadItemId = fashionCheck.HeadItemId,
                HeadGlamourId = fashionCheck.HeadGlamourId,
                HeadDye1 = fashionCheck.HeadShade1 != 0 ? fashionCheck.HeadShade1 : null,
                HeadDye2 = fashionCheck.HeadShade2 != 0 ? fashionCheck.HeadShade2 : null,
                HeadTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.HeadThemeName).RowId,
                HeadPicture = fashionCheck.HeadPicture,
                HeadPictureInfo = fashionCheck.HeadPictureInfo,
                BodyItemId = fashionCheck.BodyItemId,
                BodyGlamourId = fashionCheck.BodyGlamourId,
                BodyDye1 = fashionCheck.BodyShade1 != 0 ? fashionCheck.BodyShade1 : null,
                BodyDye2 = fashionCheck.BodyShade2 != 0 ? fashionCheck.BodyShade2 : null,
                BodyTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.BodyThemeName).RowId,
                BodyPicture = fashionCheck.BodyPicture,
                BodyPictureInfo = fashionCheck.BodyPictureInfo,
                GlovesItemId = fashionCheck.HandsItemId,
                GlovesGlamourId = fashionCheck.HandsGlamourId,
                GlovesDye1 = fashionCheck.HandsShade1 != 0 ? fashionCheck.HandsShade1 : null,
                GlovesDye2 = fashionCheck.HandsShade2 != 0 ? fashionCheck.HandsShade2 : null,
                GlovesTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.GlovesThemeName).RowId,
                GlovesPicture = fashionCheck.HandsPicture,
                GlovesPictureInfo = fashionCheck.HandsPictureInfo,
                LegsItemId = fashionCheck.LegsItemId,
                LegsGlamourId = fashionCheck.LegsGlamourId,
                LegsDye1 = fashionCheck.LegsShade1 != 0 ? fashionCheck.LegsShade1 : null,
                LegsDye2 = fashionCheck.LegsShade2 != 0 ? fashionCheck.LegsShade2 : null,
                LegsTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.LegsThemeName).RowId,
                LegsPicture = fashionCheck.LegsPicture,
                LegsPictureInfo = fashionCheck.LegsPictureInfo,
                BootsItemId = fashionCheck.FeetItemId,
                BootsGlamourId = fashionCheck.FeetGlamourId,
                BootsDye1 = fashionCheck.FeetShade1 != 0 ? fashionCheck.FeetShade1 : null,
                BootsDye2 = fashionCheck.FeetShade2 != 0 ? fashionCheck.FeetShade2 : null,
                BootsTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.BootsThemeName).RowId,
                BootsPicture = fashionCheck.FeetPicture,
                BootsPictureInfo = fashionCheck.FeetPictureInfo,
                EarringsItemId = fashionCheck.EarringsItemId,
                EarringsGlamourId = fashionCheck.EarringsGlamourId,
                EarringsTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.EarringsThemeName).RowId,
                EarringsPicture = fashionCheck.EarringsPicture,
                EarringsPictureInfo = fashionCheck.EarringsPictureInfo,
                NecklaceItemId = fashionCheck.NeckItemId,
                NecklaceGlamourId = fashionCheck.NeckGlamourId,
                NecklaceTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.NecklaceThemeName).RowId,
                NecklacePicture = fashionCheck.NeckPicture,
                NecklacePictureInfo = fashionCheck.NeckPictureInfo,
                BraceletItemId = fashionCheck.WristItemId,
                BraceletGlamourId = fashionCheck.WristGlamourId,
                BraceletTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.BraceletThemeName).RowId,
                BraceletPicture = fashionCheck.WristPicture,
                BraceletPictureInfo = fashionCheck.WristPictureInfo,
                RightRingItemId = fashionCheck.RightRingItemId,
                RightRingGlamourId = fashionCheck.RightRingGlamourId,
                RightRingTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.RightRingThemeName).RowId,
                RightRingPicture = fashionCheck.RightRingPicture,
                RightRingPictureInfo = fashionCheck.RightRingPictureInfo,
                LeftRingItemId = fashionCheck.LeftRingItemId,
                LeftRingGlamourId = fashionCheck.LeftRingGlamourId,
                LeftRingTheme = SERVICES.AllFashionThemeCategories.First(x => x.Name == SERVICES.frdata.LeftRingThemeName).RowId,
                LeftRingPicture = fashionCheck.LeftRingPicture,
                LeftRingPictureInfo = fashionCheck.LeftRingPictureInfo
            };
            await GoogleSheetWriter.InsertFashionReportDye(dyeEntry);
        }
        catch (Exception ex) { LOG.Error($"Error in OnFashionCheck: {ex}"); }
    }
}