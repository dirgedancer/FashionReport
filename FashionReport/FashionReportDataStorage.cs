using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Configuration;
using FashionReport;
using Microsoft.Extensions.DependencyInjection;

[Serializable]
public class FashionReportDataStorage : IPluginConfiguration
{
    public int Version { get; set; }
    public bool IsLongDisplay { get; set; } = true;

    public uint Week { get; set; }
    public uint WeeklyTheme { get; set; }
    public string WeeklyThemeName { get; set; } = string.Empty;

    public uint? Weapon { get; set; }
    public uint? Head { get; set; }
    public uint? Body { get; set; }
    public uint? Gloves { get; set; }
    public uint? Legs { get; set; }
    public uint? Boots { get; set; }
    public uint? Earrings { get; set; }
    public uint? Necklace { get; set; }
    public uint? Bracelet { get; set; }
    public uint? RightRing { get; set; }
    public uint? LeftRing { get; set; }

    public List<uint>? WeaponData { get; set; }
    public List<uint>? HeadData { get; set; }
    public List<uint>? BodyData { get; set; }
    public List<uint>? GlovesData { get; set; }
    public List<uint>? LegsData { get; set; }
    public List<uint>? BootsData { get; set; }
    public List<uint>? EarringsData { get; set; }
    public List<uint>? NecklaceData { get; set; }
    public List<uint>? BraceletData { get; set; }
    public List<uint>? RightRingData { get; set; }
    public List<uint>? LeftRingData { get; set; }

    public uint? WeaponDye { get; set; }
    public uint? HeadDye { get; set; }
    public uint? BodyDye { get; set; }
    public uint? GlovesDye { get; set; }
    public uint? LegsDye { get; set; }
    public uint? BootsDye { get; set; }

    public ulong Timestamp { get; set; }

    public string WeaponThemeName { get; set; } = string.Empty;
    public string HeadThemeName { get; set; } = string.Empty;
    public string BodyThemeName { get; set; } = string.Empty;
    public string GlovesThemeName { get; set; } = string.Empty;
    public string LegsThemeName { get; set; } = string.Empty;
    public string BootsThemeName { get; set; } = string.Empty;
    public string EarringsThemeName { get; set; } = string.Empty;
    public string NecklaceThemeName { get; set; } = string.Empty;
    public string BraceletThemeName { get; set; } = string.Empty;
    public string RightRingThemeName { get; set; } = string.Empty;
    public string LeftRingThemeName { get; set; } = string.Empty;

    public string WeaponDyeName { get; set; } = string.Empty;
    public string HeadDyeName { get; set; } = string.Empty;
    public string BodyDyeName { get; set; } = string.Empty;
    public string GlovesDyeName { get; set; } = string.Empty;
    public string LegsDyeName { get; set; } = string.Empty;
    public string BootsDyeName { get; set; } = string.Empty;

    public async Task SaveToDatabase()
    {
        if (await GoogleSheetData.IsWeekUpdated(Week)) { LOG.Info($"FashionReport week {Week} already updated."); return; }
        await GoogleSheetWriter.InsertFashionReport(Week, WeeklyTheme, Weapon, Head, Body, Gloves, Legs, Boots, Earrings, Necklace, Bracelet, RightRing, LeftRing, (ulong)new DateTimeOffset(new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc).AddDays(((DateTime.UtcNow - new DateTime(2018, 1, 23, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7) * 7)).ToUnixTimeSeconds(), WeaponDye, HeadDye, BodyDye, GlovesDye, LegsDye, BootsDye);
        LOG.Info($"FashionReport week {Week} has been updated to the database.");
    }

    public async Task ProcessFromId()
    {
        WeeklyThemeName = WeeklyTheme != 0 ? SERVICES.AllWeeklyFashionThemes.Find(x => x.RowId == WeeklyTheme).Name.ToString() : string.Empty;
        WeaponThemeName = Weapon != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Weapon).Name.ToString() : string.Empty;
        HeadThemeName = Head != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Head).Name.ToString() : string.Empty;
        BodyThemeName = Body != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Body).Name.ToString() : string.Empty;
        GlovesThemeName = Gloves != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Gloves).Name.ToString() : string.Empty;
        LegsThemeName = Legs != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Legs).Name.ToString() : string.Empty;
        BootsThemeName = Boots != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Boots).Name.ToString() : string.Empty;
        EarringsThemeName = Earrings != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Earrings).Name.ToString() : string.Empty;
        NecklaceThemeName = Necklace != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Necklace).Name.ToString() : string.Empty;
        BraceletThemeName = Bracelet != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == Bracelet).Name.ToString() : string.Empty;
        RightRingThemeName = RightRing != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == RightRing).Name.ToString() : string.Empty;
        LeftRingThemeName = LeftRing != null ? SERVICES.AllFashionThemeCategories.Find(x => x.RowId == LeftRing).Name.ToString() : string.Empty;
        WeaponDyeName = WeaponDye != null ? SERVICES.StainTable.FirstOrDefault(x => x.Key == WeaponDye).Value.Name.ToString() : string.Empty;
        HeadDyeName = HeadDye != null ? SERVICES.StainTable.FirstOrDefault(x => x.Key == HeadDye).Value.Name.ToString() : string.Empty;
        BodyDyeName = BodyDye != null ? SERVICES.StainTable.FirstOrDefault(x => x.Key == BodyDye).Value.Name.ToString() : string.Empty;
        GlovesDyeName = GlovesDye != null ? SERVICES.StainTable.FirstOrDefault(x => x.Key == GlovesDye).Value.Name.ToString() : string.Empty;
        LegsDyeName = LegsDye != null ? SERVICES.StainTable.FirstOrDefault(x => x.Key == LegsDye).Value.Name.ToString() : string.Empty;
        BootsDyeName = BootsDye != null ? SERVICES.StainTable.FirstOrDefault(x => x.Key == BootsDye).Value.Name.ToString() : string.Empty;
        await GetData();
    }

    internal async Task ProcessorFromString()
    {
        WeeklyTheme = !string.IsNullOrEmpty(WeeklyThemeName) ? SERVICES.AllWeeklyFashionThemes.Find(x => x.Name == WeeklyThemeName).RowId : 0;
        Weapon = !string.IsNullOrEmpty(WeaponThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == WeaponThemeName).RowId : (uint?)null;
        Head = !string.IsNullOrEmpty(HeadThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == HeadThemeName).RowId : (uint?)null;
        Body = !string.IsNullOrEmpty(BodyThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == BodyThemeName).RowId : (uint?)null;
        Gloves = !string.IsNullOrEmpty(GlovesThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == GlovesThemeName).RowId : (uint?)null;
        Legs = !string.IsNullOrEmpty(LegsThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == LegsThemeName).RowId : (uint?)null;
        Boots = !string.IsNullOrEmpty(BootsThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == BootsThemeName).RowId : (uint?)null;
        Earrings = !string.IsNullOrEmpty(EarringsThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == EarringsThemeName).RowId : (uint?)null;
        Necklace = !string.IsNullOrEmpty(NecklaceThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == NecklaceThemeName).RowId : (uint?)null;
        Bracelet = !string.IsNullOrEmpty(BraceletThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == BraceletThemeName).RowId : (uint?)null;
        RightRing = !string.IsNullOrEmpty(RightRingThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == RightRingThemeName).RowId : (uint?)null;
        LeftRing = !string.IsNullOrEmpty(LeftRingThemeName) ? SERVICES.AllFashionThemeCategories.Find(x => x.Name == LeftRingThemeName).RowId : (uint?)null;
        WeaponDye = !string.IsNullOrEmpty(WeaponDyeName) ? SERVICES.StainTable.FirstOrDefault(x => x.Value.Name.ToString() == WeaponDyeName).Key : (uint?)null;
        HeadDye = !string.IsNullOrEmpty(HeadDyeName) ? SERVICES.StainTable.FirstOrDefault(x => x.Value.Name.ToString() == HeadDyeName).Key : (uint?)null;
        BodyDye = !string.IsNullOrEmpty(BodyDyeName) ? SERVICES.StainTable.FirstOrDefault(x => x.Value.Name.ToString() == BodyDyeName).Key : (uint?)null;
        GlovesDye = !string.IsNullOrEmpty(GlovesDyeName) ? SERVICES.StainTable.FirstOrDefault(x => x.Value.Name.ToString() == GlovesDyeName).Key : (uint?)null;
        LegsDye = !string.IsNullOrEmpty(LegsDyeName) ? SERVICES.StainTable.FirstOrDefault(x => x.Value.Name.ToString() == LegsDyeName).Key : (uint?)null;
        BootsDye = !string.IsNullOrEmpty(BootsDyeName) ? SERVICES.StainTable.FirstOrDefault(x => x.Value.Name.ToString() == BootsDyeName).Key : (uint?)null;
        await GetData();
    }

    public async Task GetData()
    {
        WeaponData = Weapon != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Weapon, 1) : null;
        HeadData = Head != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Head, 3) : null;
        BodyData = Body != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Body, 4) : null;
        GlovesData = Gloves != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Gloves, 5) : null;
        LegsData = Legs != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Legs, 7) : null;
        BootsData = Boots != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Boots, 8) : null;
        EarringsData = Earrings != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Earrings, 9) : null;
        NecklaceData = Necklace != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Necklace, 10) : null;
        BraceletData = Bracelet != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)Bracelet, 11) : null;
        RightRingData = RightRing != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)RightRing, 12) : null;
        LeftRingData = LeftRing != null ? await GoogleSheetData.GetThemeItemsForSlot((uint)LeftRing, 12) : null;
        Save();
    }

    public async Task ReadAddonData(FashionCheck fashionCheck)
    {
        if (fashionCheck == null || !fashionCheck.IsReady) return;
        for (int i = 0; i < 50 && fashionCheck.AtkCount < 124; i++) await Task.Delay(100);
        if (fashionCheck.AtkCount < 124) { LOG.Warning("Timed out waiting for FashionCheck AtkValues to load."); return; }

        LOG.Debug("Reading FashionCheck addon data...");
        Week = FashionReportPoller.CurrentWeek;
        WeeklyTheme = SERVICES.AllWeeklyFashionThemes.FirstOrDefault(x => x.Name == fashionCheck.WeeklyTheme).RowId;
        WeeklyThemeName = SERVICES.AllWeeklyFashionThemes.FirstOrDefault(x => x.RowId == WeeklyTheme).Name.ToString() ?? string.Empty;
        WeaponThemeName = fashionCheck.WeaponTheme;
        HeadThemeName = fashionCheck.HeadTheme;
        BodyThemeName = fashionCheck.BodyTheme;
        GlovesThemeName = fashionCheck.HandsTheme;
        LegsThemeName = fashionCheck.LegsTheme;
        BootsThemeName = fashionCheck.FeetTheme;
        EarringsThemeName = fashionCheck.EarringsTheme;
        NecklaceThemeName = fashionCheck.NeckTheme;
        BraceletThemeName = fashionCheck.WristTheme;
        RightRingThemeName = fashionCheck.RightRingTheme;
        LeftRingThemeName = fashionCheck.LeftRingTheme;
        await ProcessorFromString();
        WeaponDye = HeadDye = BodyDye = GlovesDye = LegsDye = BootsDye = null;
        WeaponDyeName = HeadDyeName = BootsDyeName = GlovesDyeName = LegsDyeName = BootsDyeName = string.Empty;
        Timestamp = (ulong)new DateTimeOffset(FashionReportPoller.GetFridayOfDyeWeek(Week)).ToUnixTimeSeconds();
    }
    public static FashionReportDataStorage Load() => SERVICES.Interface.GetPluginConfig() as FashionReportDataStorage ?? new FashionReportDataStorage();
    public void Save() => SERVICES.Interface.SavePluginConfig(this);
}
