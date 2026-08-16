using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;
using static FashionReportCalculator.EquippedGearService;
#pragma warning disable IDE1006


namespace FashionReportCalculator;

public class DisplayWindow : Window, IDisposable
{
    private readonly Dictionary<string, uint> SlotMax = new() { { "Weapon", 10 }, { "Head", 10 }, { "Body", 10 }, { "Gloves", 10 }, { "Legs", 10 }, { "Boots", 10 }, { "Earrings", 8 }, { "Necklace", 8 }, { "Bracelet", 8 }, { "RightRing", 8 }, { "LeftRing", 8 } };
    private Vector2 _windowSize = new Vector2(1000, 200);
    private IDalamudTextureWrap? CauldronTexture;
    private IDalamudTextureWrap? ShortTexture;
    private IDalamudTextureWrap? LongTexture;
    private IDalamudTextureWrap? AboutTexture;
    private readonly Vector4[] IconColors = new Vector4[9] { new Vector4(0f, 0f, 1f, 1f), new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 1f, 0f, 1f), new Vector4(0.5f, 0.5f, 0.5f, 1f), new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector4(0.4f, 0.2f, 0f, 1f), new Vector4(0f, 1f, 0f, 1f), new Vector4(1f, 1f, 1f, 1f), new Vector4(0.5f, 0f, 0.5f, 1f) };
    private readonly float slotWidth;
    private readonly float themeWidth;
    private readonly float pointsWidth;
    private IFontHandle? WeeklyThemeFontHandle = CreateFontFromResource("FashionReport.Fonts.Arcade.ttf", 100f, SERVICES.Interface.UiBuilder.FontAtlas);
    private IFontHandle? WeekFontHandle = CreateFontFromResource("FashionReport.Fonts.Arcade.ttf", 48f, SERVICES.Interface.UiBuilder.FontAtlas);
    private IFontHandle? ShortWeeklyThemeFontHandle = CreateFontFromResource("FashionReport.Fonts.Arcade.ttf", 50f, SERVICES.Interface.UiBuilder.FontAtlas);
    private IFontHandle? ShortWeekFontHandle = CreateFontFromResource("FashionReport.Fonts.Arcade.ttf", 24f, SERVICES.Interface.UiBuilder.FontAtlas);
    private string? uSlot = null;
    private string? uTheme = null;
    internal Dictionary<string, List<DisplayItem>> DisplayData = new();
    private Vector2? cacheThemeWindowSize = null;
    private float? cacheScrollableHeight = null;
    private List<DisplayItem>? cacheItems = null;

    public DisplayWindow() : base("Fashion Report Display Window", ImGuiWindowFlags.NoResize)
    {
        Size = _windowSize;
        SizeCondition = ImGuiCond.Always;
        CauldronTexture = SERVICES.Texture.CreateFromImageAsync(Assembly.GetExecutingAssembly().GetManifestResourceStream("FashionReport.images.Cauldron.png")!).Result;
        AboutTexture = SERVICES.Texture.CreateFromImageAsync(Assembly.GetExecutingAssembly().GetManifestResourceStream("FashionReport.images.About.png")!).Result;
        ShortTexture = SERVICES.Texture.CreateFromImageAsync(Assembly.GetExecutingAssembly().GetManifestResourceStream("FashionReport.images.Short.png")!).Result;
        LongTexture = SERVICES.Texture.CreateFromImageAsync(Assembly.GetExecutingAssembly().GetManifestResourceStream("FashionReport.images.Long.png")!).Result;
        float padding = ImGui.GetStyle().CellPadding.X * 2;
        slotWidth = ImGui.CalcTextSize("Right Ring (Metallic Cobalt Green)").X + padding;
        themeWidth = ImGui.CalcTextSize("Fashionably Late Allagan").X + padding;
        pointsWidth = Math.Max(
            ImGui.CalcTextSize("Points").X,
            ImGui.CalcTextSize("100").X)
            + (padding * 2)
            + 12f;        
        RefreshDisplayData();
    }

    public override void Draw()
    {
        Size = _windowSize;
        SizeCondition = ImGuiCond.Always;
        DrawWeeklyHeader();
        DrawTopButtons();
        (float TableWidth, float TableHeight) = GeneralData.IsLongDisplay ? DrawLongTable() : DrawShortTable();
        Vector2 WindowPadding = ImGui.GetStyle().WindowPadding;
        const float tableTop = 130f;

        _windowSize = new Vector2(
            TableWidth + (WindowPadding.X * 2),
            tableTop + TableHeight + (WindowPadding.Y * 2) + 8f);        if (uSlot != null)

        DrawThemeWindow();
    }

    private void DrawWeeklyHeader()
    {
        string weeklyTheme = SERVICES.frdata.WeeklyThemeName ?? string.Empty;
        string weekText = SERVICES.frdata.Week.ToString();
        bool isOldWeek = SERVICES.frdata.Week != FashionReportPoller.CurrentWeek;
        IFontHandle? themeFont = GeneralData.IsLongDisplay ? WeeklyThemeFontHandle : ShortWeeklyThemeFontHandle;
        IFontHandle? weekFont = GeneralData.IsLongDisplay ? WeekFontHandle : ShortWeekFontHandle;
        if (isOldWeek) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
        if (themeFont != null) themeFont.Push();
        Vector2 themeSize = ImGui.CalcTextSize(weeklyTheme);
        ImGui.SetCursorPosX((_windowSize.X - themeSize.X) / 2);
        ImGui.SetCursorPosY(GeneralData.IsLongDisplay ? 15f : 35f);
        ImGui.Text(weeklyTheme);
        if (themeFont != null) themeFont.Pop();
        if (weekFont != null) weekFont.Push();
        Vector2 weekSize = ImGui.CalcTextSize(weekText);
        ImGui.SetCursorPosX((_windowSize.X - weekSize.X) / 2);
        ImGui.SetCursorPosY(GeneralData.IsLongDisplay ? 10f + themeSize.Y - 30f : 130f - weekSize.Y - 20f);
        ImGui.Text(weekText);
        if (weekFont != null) weekFont.Pop();
        if (isOldWeek) ImGui.PopStyleColor();
    }

    private void DrawTopButtons()
    {
        Vector2 windowSize = ImGui.GetWindowSize();
        Vector2 padding = ImGui.GetStyle().WindowPadding;
        Vector2 size = new Vector2(100f, 100f);
        if (CauldronTexture != null)
        {
            ImGui.SetCursorPos(new Vector2(windowSize.X - size.X, 30));
            ImGui.Image(CauldronTexture.Handle, size);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Brew Donations (Donate to help)");
            if (ImGui.IsItemClicked()) Dalamud.Utility.Util.OpenLink("https://ko-fi.com/theredheadedwitch");
        }
        if (AboutTexture != null)
        {
            Vector2 aboutSize = new Vector2(size.X / 2, size.Y / 2);
            float aboutXPos = windowSize.X - size.X - aboutSize.X;
            float aboutYPos = 30 + size.Y - aboutSize.Y;
            ImGui.SetCursorPos(new Vector2(aboutXPos, aboutYPos));
            ImGui.Image(AboutTexture.Handle, aboutSize);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("About this plugin");
            if (ImGui.IsItemClicked())
                FashionReport.AboutWindow.IsOpen = true;
        }
        else
            LOG.Error("AboutTexture is null");
        IDalamudTextureWrap? SwitchTexture = GeneralData.IsLongDisplay ? ShortTexture : LongTexture;
        if (SwitchTexture != null)
        {
            ImGui.SetCursorPos(new Vector2(10, 30 + size.Y - (SwitchTexture!.Height / 10)));
            string tooltipText = GeneralData.IsLongDisplay ? "Switch to Short View" : "Switch to Long View";
            ImGui.Image(SwitchTexture.Handle, new Vector2(SwitchTexture.Width / 10, SwitchTexture.Height / 10));
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(tooltipText);
            if (ImGui.IsItemClicked())
                GeneralData.IsLongDisplay = !GeneralData.IsLongDisplay;
        }
        else
            LOG.Error("SwitchTexture is null");
        ImGui.Spacing();
    }

    private (float, float) DrawShortTable()
    {
        ImGui.SetCursorPosY(130);
        uint TotalSlot = 0, TotalDye = 0, GrandTotal = 0;
        float contentStart = 130;
        float tableWidth = slotWidth + themeWidth + (pointsWidth * 3) + (ImGui.GetStyle().ItemSpacing.X * 4) + 5;
        if (ImGui.BeginTable("ShortTable", 5, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit, new Vector2(tableWidth, 0)))
        {
            ImGui.TableSetupColumn("Slot", ImGuiTableColumnFlags.WidthFixed, slotWidth);
            ImGui.TableSetupColumn("Theme", ImGuiTableColumnFlags.WidthFixed, themeWidth);
            ImGui.TableSetupColumn("Slot\nPoints", ImGuiTableColumnFlags.WidthFixed, pointsWidth);
            ImGui.TableSetupColumn("Dye\nPoints", ImGuiTableColumnFlags.WidthFixed, pointsWidth);
            ImGui.TableSetupColumn("Total", ImGuiTableColumnFlags.WidthFixed, pointsWidth);
            DrawCenteredHeaders("Slot", "Theme", "Slot\nPoints", "Dye\nPoints", "Total");

            foreach (string slot in SERVICES.FRSlots)
            {
                ImGui.TableNextColumn();
                uint? dyeId = slot switch
                {
                    "Weapon" => SERVICES.frdata.WeaponDye,
                    "Head" => SERVICES.frdata.HeadDye,
                    "Body" => SERVICES.frdata.BodyDye,
                    "Gloves" => SERVICES.frdata.GlovesDye,
                    "Legs" => SERVICES.frdata.LegsDye,
                    "Boots" => SERVICES.frdata.BootsDye,
                    _ => null
                };
                string dyeText = "";
                Vector4 color = Vector4.One;
                if (dyeId != null && dyeId != 0 && SERVICES.StainTable.TryGetValue(dyeId.Value, out (string Name, uint ItemId, uint IconId) stain))
                {
                    dyeText = " (" + stain.Name + ")";
                    color = GetIconColor(stain.IconId);
                }
                Vector2 textSize = ImGui.CalcTextSize(slot + dyeText);
                float offsetX = (ImGui.GetColumnWidth() - textSize.X) / 2;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offsetX);
                ImGui.TextUnformatted(slot);
                if (!string.IsNullOrEmpty(dyeText))
                {
                    ImGui.SameLine();
                    ImGui.PushStyleColor(ImGuiCol.Text, color);
                    ImGui.TextUnformatted(dyeText);
                    ImGui.PopStyleColor();
                }
                ImGui.TableNextColumn();
                string themeName = slot switch
                {
                    "Weapon" => SERVICES.frdata.WeaponThemeName,
                    "Head" => SERVICES.frdata.HeadThemeName,
                    "Body" => SERVICES.frdata.BodyThemeName,
                    "Gloves" => SERVICES.frdata.GlovesThemeName,
                    "Legs" => SERVICES.frdata.LegsThemeName,
                    "Boots" => SERVICES.frdata.BootsThemeName,
                    "Earrings" => SERVICES.frdata.EarringsThemeName,
                    "Necklace" => SERVICES.frdata.NecklaceThemeName,
                    "Bracelet" => SERVICES.frdata.BraceletThemeName,
                    "RightRing" => SERVICES.frdata.RightRingThemeName,
                    "LeftRing" => SERVICES.frdata.LeftRingThemeName,
                    _ => ""
                };
                TableCellTheme(slot, themeName);
                ImGui.TableNextColumn();
                uint slotPoints = (uint)CalculateSlotPoints(slot);
                TableCellCenteredText(slotPoints.ToString());
                TotalSlot += slotPoints;
                ImGui.TableNextColumn();
                uint dyePoints = 0;
                if (slot != "Earrings" && slot != "Necklace" && slot != "Bracelet" && slot != "RightRing" && slot != "LeftRing")
                    dyePoints = (uint)(CalculateDyePoints(slot) ?? 0);
                TableCellCenteredText(dyePoints > 0 ? dyePoints.ToString() : "");
                TotalDye += dyePoints;
                ImGui.TableNextColumn();
                uint totalPoints = slotPoints + dyePoints;
                TableCellCenteredText(totalPoints.ToString());
                GrandTotal += totalPoints;
                ImGui.TableNextRow();
            }
            ImGui.TableNextColumn();
            TableCellCenteredText("\nTotals");
            ImGui.TableNextColumn();
            TableCellCenteredText("");
            ImGui.TableNextColumn();
            TableCellCenteredText("\n" + TotalSlot.ToString());
            ImGui.TableNextColumn();
            TableCellCenteredText("\n" + TotalDye.ToString());
            ImGui.TableNextColumn();
            Vector4 color2;
            if (GrandTotal < 80)
                color2 = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            else if (GrandTotal >= 80 && GrandTotal < 100)
                color2 = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            else
                color2 = new Vector4(1.0f, 0.843f, 0.0f, 1.0f);
            ImGui.PushStyleColor(ImGuiCol.Text, color2);
            TableCellCenteredText("\n" + GrandTotal.ToString());
            ImGui.PopStyleColor();
            ImGui.EndTable();
        }
        float contentEnd = ImGui.GetCursorPosY();
        return (tableWidth, contentEnd - contentStart);
    }

    private (float, float) DrawLongTable()
    {
        ImGui.SetCursorPosY(130);
        uint TotalSlot = 0, TotalDye = 0, TotalPoints = 0, MaxPoints = 0;
        float contentStart = 130;
        string[] requiredDyes =
        {
            SERVICES.frdata.WeaponDyeName,
            SERVICES.frdata.HeadDyeName,
            SERVICES.frdata.BodyDyeName,
            SERVICES.frdata.GlovesDyeName,
            SERVICES.frdata.LegsDyeName,
            SERVICES.frdata.BootsDyeName
        };

        string[] themes =
        {
            SERVICES.frdata.WeaponThemeName,
            SERVICES.frdata.HeadThemeName,
            SERVICES.frdata.BodyThemeName,
            SERVICES.frdata.GlovesThemeName,
            SERVICES.frdata.LegsThemeName,
            SERVICES.frdata.BootsThemeName,
            SERVICES.frdata.EarringsThemeName,
            SERVICES.frdata.NecklaceThemeName,
            SERVICES.frdata.BraceletThemeName,
            SERVICES.frdata.RightRingThemeName,
            SERVICES.frdata.LeftRingThemeName
        };

        string[] gearNames = SERVICES.FRSlots
            .Select(slot =>
                EquippedGearService.CurrentEquippedGear[slot].Name ?? string.Empty)
            .ToArray();

        string[] itemDyes = SERVICES.FRSlots
            .SelectMany(slot =>
            {
                EquippedItemData gear =
                    EquippedGearService.CurrentEquippedGear[slot];

                return new[]
                {
                    gear.Stain1 ?? string.Empty,
                    gear.Stain2 ?? string.Empty
                };
            })
            .ToArray();

        float longSlotWidth =
            MeasureColumnWidth(SERVICES.FRSlots, "Slot", 4f);

        float longDyesWidth =
            MeasureColumnWidth(requiredDyes, "Dyes", 4f);

        float longThemeWidth =
            MeasureColumnWidth(themes, "Theme", 4f);

        float longGearWidth =
            MeasureColumnWidth(gearNames, "Gear", 8f);

        float longItemDyeWidth =
            MeasureColumnWidth(itemDyes, "Dye 1", 8f);

        const int columnCount = 10;

        float columnSpacing =
            ImGui.GetStyle().ItemSpacing.X * (columnCount - 1);

        float tableWidth =
            longSlotWidth
            + longDyesWidth
            + longThemeWidth
            + longGearWidth
            + (longItemDyeWidth * 2)
            + (pointsWidth * 4)
            + columnSpacing
            + 10f;

        if (ImGui.BeginTable("CenteredTable", 10, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit, new Vector2(tableWidth, 0)))
        {
            ImGui.TableSetupColumn(
                "Slot",
                ImGuiTableColumnFlags.WidthFixed,
                longSlotWidth);

            ImGui.TableSetupColumn(
                "Dyes",
                ImGuiTableColumnFlags.WidthFixed,
                longDyesWidth);

            ImGui.TableSetupColumn(
                "Theme",
                ImGuiTableColumnFlags.WidthFixed,
                longThemeWidth);

            ImGui.TableSetupColumn(
                "Gear",
                ImGuiTableColumnFlags.WidthFixed,
                longGearWidth);

            ImGui.TableSetupColumn(
                "Dye 1",
                ImGuiTableColumnFlags.WidthFixed,
                longItemDyeWidth);

            ImGui.TableSetupColumn(
                "Dye 2",
                ImGuiTableColumnFlags.WidthFixed,
                longItemDyeWidth);
            ImGui.TableSetupColumn("Slot\nPoints", ImGuiTableColumnFlags.WidthFixed, pointsWidth);
            ImGui.TableSetupColumn("Dye\nPoints", ImGuiTableColumnFlags.WidthFixed, pointsWidth);
            ImGui.TableSetupColumn("Total\nPoints", ImGuiTableColumnFlags.WidthFixed, pointsWidth);
            ImGui.TableSetupColumn("Max\nPoints", ImGuiTableColumnFlags.WidthFixed, pointsWidth);
            DrawCenteredHeaders(
                "Slot",
                "Dyes",
                "Theme",
                "Gear",
                "Dye 1",
                "Dye 2",
                "Slot\nPoints",
                "Dye\nPoints",
                "Total\nPoints",
                "Max\nPoints");            
            foreach (string slot in SERVICES.FRSlots)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                TableCellCenteredText(slot);
                ImGui.TableNextColumn();
                uint? dyeId = slot switch
                {
                    "Weapon" => SERVICES.frdata.WeaponDye,
                    "Head" => SERVICES.frdata.HeadDye,
                    "Body" => SERVICES.frdata.BodyDye,
                    "Gloves" => SERVICES.frdata.GlovesDye,
                    "Legs" => SERVICES.frdata.LegsDye,
                    "Boots" => SERVICES.frdata.BootsDye,
                    _ => 0u
                };
                string dyeText = "-";
                Vector4 color = Vector4.One;
                if (dyeId != null && dyeId != 0 && SERVICES.StainTable.TryGetValue(dyeId.Value, out (string Name, uint ItemId, uint IconId) stain))
                {
                    dyeText = stain.Name;
                    color = GetIconColor(stain.IconId);
                    ImGui.PushStyleColor(ImGuiCol.Text, color);
                    TableCellCenteredText(dyeText);
                    ImGui.PopStyleColor();
                }
                ImGui.TableNextColumn();
                string themeName = slot switch
                {
                    "Weapon" => SERVICES.frdata.WeaponThemeName,
                    "Head" => SERVICES.frdata.HeadThemeName,
                    "Body" => SERVICES.frdata.BodyThemeName,
                    "Gloves" => SERVICES.frdata.GlovesThemeName,
                    "Legs" => SERVICES.frdata.LegsThemeName,
                    "Boots" => SERVICES.frdata.BootsThemeName,
                    "Earrings" => SERVICES.frdata.EarringsThemeName,
                    "Necklace" => SERVICES.frdata.NecklaceThemeName,
                    "Bracelet" => SERVICES.frdata.BraceletThemeName,
                    "RightRing" => SERVICES.frdata.RightRingThemeName,
                    "LeftRing" => SERVICES.frdata.LeftRingThemeName,
                    _ => ""
                };
                TableCellTheme(slot, themeName);
                ImGui.TableNextColumn();
                TableCellCenteredText(EquippedGearService.CurrentEquippedGear[slot].Name ?? "");
                ImGui.TableNextColumn();
                EquippedItemData gear = EquippedGearService.CurrentEquippedGear[slot];
                string dyeText1 = "";
                Vector4 color1 = Vector4.One;
                if (gear.StainId1 != 0)
                {
                    dyeText1 = gear.Stain1;
                    color1 = GetIconColor(gear.StainIcon1);
                }
                ImGui.PushStyleColor(ImGuiCol.Text, color1);
                TableCellCenteredText(dyeText1);
                ImGui.PopStyleColor();
                ImGui.TableNextColumn();
                string dyeText2 = "";
                Vector4 color2 = Vector4.One;
                if (gear.StainId2 != 0)
                {
                    dyeText2 = gear.Stain2;
                    color2 = GetIconColor(gear.StainIcon2);
                }
                ImGui.PushStyleColor(ImGuiCol.Text, color2);
                TableCellCenteredText(dyeText2);
                ImGui.PopStyleColor();
                ImGui.TableNextColumn();
                uint slotPoints = (uint)CalculateSlotPoints(slot);
                TableCellCenteredText(slotPoints.ToString());
                TotalSlot += slotPoints;
                ImGui.TableNextColumn();
                uint dyePoints = 0;
                if (slot != "Earrings" && slot != "Necklace" && slot != "Bracelet" && slot != "RightRing" && slot != "LeftRing")
                    dyePoints = (uint)(CalculateDyePoints(slot) ?? 0);
                TableCellCenteredText(dyePoints.ToString());
                TotalDye += dyePoints;
                ImGui.TableNextColumn();
                uint totalPoints = slotPoints + dyePoints;
                TableCellCenteredText(totalPoints.ToString());
                TotalPoints += totalPoints;
                ImGui.TableNextColumn();
                uint maxPoints = SlotMax.ContainsKey(slot) ? SlotMax[slot] : 0;
                TableCellCenteredText(maxPoints.ToString());
                MaxPoints += maxPoints;
            }
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            TableCellCenteredText("\nTotals");
            ImGui.TableNextColumn();
            TableCellCenteredText("");
            ImGui.TableNextColumn();
            TableCellCenteredText("");
            ImGui.TableNextColumn();
            TableCellCenteredText("");
            ImGui.TableNextColumn();
            TableCellCenteredText("");
            ImGui.TableNextColumn();
            TableCellCenteredText("");
            ImGui.TableNextColumn();
            TableCellCenteredText("\n" + TotalSlot.ToString());
            ImGui.TableNextColumn();
            TableCellCenteredText("\n" + TotalDye.ToString());
            ImGui.TableNextColumn();
            Vector4 color3 = new Vector4();
            if (TotalPoints < 80) color3 = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            else if (TotalPoints >= 80 && TotalPoints < 100) color3 = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            else color3 = new Vector4(1.0f, 0.843f, 0.0f, 1.0f);
            ImGui.PushStyleColor(ImGuiCol.Text, color3);
            TableCellCenteredText("\n" + TotalPoints.ToString());
            ImGui.PopStyleColor();
            ImGui.TableNextColumn();
            TableCellCenteredText("\n" + MaxPoints.ToString());
            ImGui.EndTable();
        }
        float contentEnd = ImGui.GetCursorPosY();
        return (tableWidth, contentEnd - contentStart);
    }

    private int CalculateSlotPoints(string slot)
    {
        uint id = 0;
        if (slot == "Weapon") id = EquippedGearService.Weapon.GlamourId != 0 ? EquippedGearService.Weapon.GlamourId : EquippedGearService.Weapon.ItemId;
        else if (slot == "Head") id = EquippedGearService.Head.GlamourId != 0 ? EquippedGearService.Head.GlamourId : EquippedGearService.Head.ItemId;
        else if (slot == "Body") id = EquippedGearService.Body.GlamourId != 0 ? EquippedGearService.Body.GlamourId : EquippedGearService.Body.ItemId;
        else if (slot == "Gloves") id = EquippedGearService.Gloves.GlamourId != 0 ? EquippedGearService.Gloves.GlamourId : EquippedGearService.Gloves.ItemId;
        else if (slot == "Legs") id = EquippedGearService.Legs.GlamourId != 0 ? EquippedGearService.Legs.GlamourId : EquippedGearService.Legs.ItemId;
        else if (slot == "Boots") id = EquippedGearService.Boots.GlamourId != 0 ? EquippedGearService.Boots.GlamourId : EquippedGearService.Boots.ItemId;
        else if (slot == "Earrings") id = EquippedGearService.Earrings.GlamourId != 0 ? EquippedGearService.Earrings.GlamourId : EquippedGearService.Earrings.ItemId;
        else if (slot == "Necklace") id = EquippedGearService.Necklace.GlamourId != 0 ? EquippedGearService.Necklace.GlamourId : EquippedGearService.Necklace.ItemId;
        else if (slot == "Bracelet") id = EquippedGearService.Bracelet.GlamourId != 0 ? EquippedGearService.Bracelet.GlamourId : EquippedGearService.Bracelet.ItemId;
        else if (slot == "RightRing") id = EquippedGearService.RightRing.GlamourId != 0 ? EquippedGearService.RightRing.GlamourId : EquippedGearService.RightRing.ItemId;
        else if (slot == "LeftRing") id = EquippedGearService.LeftRing.GlamourId != 0 ? EquippedGearService.LeftRing.GlamourId : EquippedGearService.LeftRing.ItemId;
        if (id == 0) return 0;
        List<uint>? theme = slot switch { "Weapon" => SERVICES.frdata.WeaponData, "Head" => SERVICES.frdata.HeadData, "Body" => SERVICES.frdata.BodyData, "Gloves" => SERVICES.frdata.GlovesData, "Legs" => SERVICES.frdata.LegsData, "Boots" => SERVICES.frdata.BootsData, "Earrings" => SERVICES.frdata.EarringsData, "Necklace" => SERVICES.frdata.NecklaceData, "Bracelet" => SERVICES.frdata.BraceletData, "RightRing" => SERVICES.frdata.RightRingData, "LeftRing" => SERVICES.frdata.LeftRingData, _ => null };
        return theme == null || theme.Count == 0 ? (int)SlotMax[slot] : theme.Contains(id) ? (int)SlotMax[slot] : 2;
    }

    private int? CalculateDyePoints(string slot)
    {
        if (slot != "Weapon" && slot != "Head" && slot != "Body" && slot != "Gloves" && slot != "Legs" && slot != "Boots") return null;
        uint? required = slot switch { "Weapon" => SERVICES.frdata.WeaponDye, "Head" => SERVICES.frdata.HeadDye, "Body" => SERVICES.frdata.BodyDye, "Gloves" => SERVICES.frdata.GlovesDye, "Legs" => SERVICES.frdata.LegsDye, "Boots" => SERVICES.frdata.BootsDye, _ => null };
        if (required == null || required == 0) return null;
        if (EquippedGearService.CurrentEquippedGear[slot].StainId1 == required || EquippedGearService.CurrentEquippedGear[slot].StainId2 == required) return 2;
        if (EquippedGearService.CurrentEquippedGear[slot].StainId1 != 0 || EquippedGearService.CurrentEquippedGear[slot].StainId2 != 0)
        {
            uint reqIcon = SERVICES.StainTable.ContainsKey(required.Value) ? SERVICES.StainTable[required.Value].IconId : 0;
            int reqGroup = reqIcon >= 22804 ? (int)((reqIcon - 22804) % 9) : -1;
            int s0Group = EquippedGearService.CurrentEquippedGear[slot].StainIcon1 >= 22804 ? (int)((EquippedGearService.CurrentEquippedGear[slot].StainIcon1 - 22804) % 9) : -1;
            int s1Group = EquippedGearService.CurrentEquippedGear[slot].StainIcon2 >= 22804 ? (int)((EquippedGearService.CurrentEquippedGear[slot].StainIcon2 - 22804) % 9) : -1;
            if (reqGroup != -1 && (reqGroup == s0Group || reqGroup == s1Group)) return 1;
        }
        return 0;
    }

    private void TableCellCenteredText(string text)
    {
        float cellWidth = ImGui.GetColumnWidth();
        float textWidth = ImGui.CalcTextSize(text).X;
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (cellWidth - textWidth) * 0.5f);
        ImGui.Text(text);
    }

    private void DrawCenteredHeaders(params string[] headers)
    {
        float maxHeaderHeight = 0;
        for (int i = 0; i < headers.Length; i++)
        {
            string[] lines = headers[i].Split('\n');
            float currentHeaderHeight = lines.Length * ImGui.GetTextLineHeight();
            if (currentHeaderHeight > maxHeaderHeight)
                maxHeaderHeight = currentHeaderHeight;
        }
        ImGui.TableNextRow(ImGuiTableRowFlags.Headers, maxHeaderHeight);
        for (int i = 0; i < headers.Length; i++)
        {
            ImGui.TableSetColumnIndex(i);
            string[] lines = headers[i].Split('\n');
            float cellWidth = ImGui.GetColumnWidth();
            float yOffset = ImGui.GetCursorPosY();
            float currentHeaderHeight = lines.Length * ImGui.GetTextLineHeight();
            if (lines.Length == 1)
                yOffset += (maxHeaderHeight - currentHeaderHeight) * 0.5f;
            for (int j = 0; j < lines.Length; j++)
            {
                float textWidth = ImGui.CalcTextSize(lines[j]).X;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (cellWidth - textWidth) * 0.5f);
                ImGui.SetCursorPosY(yOffset + j * ImGui.GetTextLineHeight());
                ImGui.Text(lines[j]);
            }
        }
    }

    internal static IFontHandle? CreateFontFromResource(string resourceName, float sizePx, IFontAtlas atlas)
    {
        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null) return null;
        try { return atlas.NewDelegateFontHandle(build => build.OnPreBuild(tk => tk.AddFontFromStream(stream, new SafeFontConfig { SizePx = sizePx }, true, resourceName))); }
        catch (Exception e)
        {
            LOG.Error($"Font load failed: {e.Message}");
            return null;
        }
    }

    void TableCellTheme(string slot, string theme)
    {
        Vector2 textSize = ImGui.CalcTextSize(theme);
        Vector2 buttonSize = new Vector2(textSize.X + 4, textSize.Y + 4);
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetColumnWidth() - buttonSize.X) * 0.5f);
        string buttonId = "##" + slot + "_btn";
        if (ImGui.InvisibleButton(buttonId, buttonSize))
        {
            cacheThemeWindowSize = null;
            uSlot = slot;
            uTheme = theme;
        }
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        uint blueColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0.5f, 1f, 1f));
        Vector2 pos = ImGui.GetItemRectMin();
        drawList.AddText(pos, blueColor, theme);
        drawList.AddLine(new Vector2(pos.X, pos.Y + textSize.Y + 1), new Vector2(pos.X + textSize.X, pos.Y + textSize.Y + 1), blueColor, 1f);
    }

    private void DrawThemeWindow()
    {
        if (!cacheThemeWindowSize.HasValue)
        {
            DisplayData.TryGetValue(uSlot!, out List<DisplayItem>? items);
            if (items == null) return;
            cacheItems = items;
            ImGui.SetWindowFontScale(2.5f);
            float TitleHeight = ImGui.CalcTextSize(uTheme).Y;
            ImGui.SetWindowFontScale(1f);
            float sep = 1.0f + 2 * ImGui.GetStyle().FramePadding.Y;
            float spac = ImGui.GetStyle().ItemSpacing.Y;
            float sp = ((2 * sep) + spac);
            cacheScrollableHeight = (((cacheItems.Count > 10 ? 10 : cacheItems.Count) * 50) + (((cacheItems.Count > 10 ? 10 : cacheItems.Count) - 1) * 12));
            cacheThemeWindowSize = new Vector2(1000f, 30 + TitleHeight + 12 + (float)cacheScrollableHeight + 15);
        }
        ImGui.SetNextWindowSize(cacheThemeWindowSize.Value, ImGuiCond.Always);
        ImGui.Begin($"Fashion Report Theme Items", ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoResize);
        ImGui.SetWindowFontScale(2.5f);
        Vector2 themeTextSize = ImGui.CalcTextSize(uTheme);
        float centerPos = (ImGui.GetContentRegionAvail().X - themeTextSize.X) * 0.5f;
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + centerPos);
        ImGui.Text(uTheme);
        ImGui.SetWindowFontScale(1f);
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.BeginChild("ItemScrollRegion", new Vector2(0, cacheScrollableHeight!.Value), false);
        uint MaxIcons = 1;
        for (int c = 0; c < cacheItems!.Count; c++)
        {
            uint iIcons = DrawItem(cacheItems[c]);
            if (iIcons + 1 > MaxIcons) MaxIcons = iIcons + 1;
            if (c + 1 < cacheItems.Count)
            {
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
            }
        }
        ImGui.EndChild();
        float buttonWidth = ImGui.CalcTextSize("Close").X + ImGui.GetStyle().FramePadding.X * 2;
        float buttonX = ImGui.GetWindowWidth() - ImGui.GetStyle().WindowPadding.X - buttonWidth;
        ImGui.SetCursorPos(new Vector2(buttonX, 30));
        if (ImGui.Button("Close"))
        {
            uSlot = null;
            cacheThemeWindowSize = null;
            cacheItems = null;
            cacheScrollableHeight = null;
        }
        ImGui.End();
    }

    uint DrawItem(DisplayItem display)
    {
        if (display.ItemId == 0) return 0;
        float yPos = ImGui.GetCursorPosY();
        float xPos = ImGui.GetCursorPosX();
        if (SERVICES.Texture.GetFromGameIcon(new GameIconLookup { IconId = display.IconId }).TryGetWrap(out IDalamudTextureWrap? icon, out _))
            if (icon != null) ImGui.Image(icon.Handle, new Vector2(50f, 50f));
        ImGui.SameLine();
        ImGui.SetWindowFontScale(2f);
        ImGui.Text(display.Name);
        ImGui.SetWindowFontScale(1f);
        ImGui.SetCursorPos(new Vector2(xPos, yPos + 30));
        ImGui.Text("\t\t\t\t\tLevel: " + display.Level);
        float rightEdge = ImGui.GetWindowContentRegionMax().X;
        List<(uint IconId, bool Show)> icons = new()
        {
            (61432, display.IsQuestReward),
            (65014, display.IsWolf),
            (60831, display.IsDuty),
            (65005, display.IsSeals),
            (65002, display.IsVendor),
            (66456, display.IsCraftable)
        };
        foreach ((uint iconId, bool show) in icons)
        {
            if (!show) continue;
            rightEdge -= 50f + 2f;
            if (SERVICES.Texture.GetFromGameIcon(new GameIconLookup { IconId = iconId }).TryGetWrap(out IDalamudTextureWrap? extraIcon, out _))
                if (extraIcon != null) { ImGui.SetCursorPos(new Vector2(rightEdge, yPos)); ImGui.Image(extraIcon.Handle, new Vector2(50f, 50f)); }
        }
        ImGui.SetCursorPosY(yPos + 50f);
        return (uint)icons.Count(i => i.Show) + 1;
    }

    private Vector4 GetIconColor(uint iconId) => IconColors[(iconId - 22804) % 9];

    public void Dispose()
    {
        CauldronTexture?.Dispose();
        ShortTexture?.Dispose();
        LongTexture?.Dispose();
        WeeklyThemeFontHandle?.Dispose();
        WeekFontHandle?.Dispose();
        ShortWeeklyThemeFontHandle?.Dispose();
        ShortWeekFontHandle?.Dispose();
    }

    internal void RefreshDisplayData()
    {
        DisplayData.Clear();

        foreach (string slot in SERVICES.FRSlots)
        {
            List<DisplayItem> slotItems = new();

            List<uint>? itemIds = slot switch
            {
                "Weapon" => SERVICES.frdata.WeaponData,
                "Head" => SERVICES.frdata.HeadData,
                "Body" => SERVICES.frdata.BodyData,
                "Gloves" => SERVICES.frdata.GlovesData,
                "Legs" => SERVICES.frdata.LegsData,
                "Boots" => SERVICES.frdata.BootsData,
                "Earrings" => SERVICES.frdata.EarringsData,
                "Necklace" => SERVICES.frdata.NecklaceData,
                "Bracelet" => SERVICES.frdata.BraceletData,
                "RightRing" => SERVICES.frdata.RightRingData,
                "LeftRing" => SERVICES.frdata.LeftRingData,
                _ => null
            };

            string themeName = slot switch
            {
                "Weapon" => SERVICES.frdata.WeaponThemeName,
                "Head" => SERVICES.frdata.HeadThemeName,
                "Body" => SERVICES.frdata.BodyThemeName,
                "Gloves" => SERVICES.frdata.GlovesThemeName,
                "Legs" => SERVICES.frdata.LegsThemeName,
                "Boots" => SERVICES.frdata.BootsThemeName,
                "Earrings" => SERVICES.frdata.EarringsThemeName,
                "Necklace" => SERVICES.frdata.NecklaceThemeName,
                "Bracelet" => SERVICES.frdata.BraceletThemeName,
                "RightRing" => SERVICES.frdata.RightRingThemeName,
                "LeftRing" => SERVICES.frdata.LeftRingThemeName,
                _ => ""
            };

            if (itemIds == null ||
                itemIds.Count == 0 ||
                string.IsNullOrEmpty(themeName))
            {
                continue;
            }

            foreach (uint itemId in itemIds)
            {
                Item item =
                    SERVICES.AllItems.FirstOrDefault(x => x.RowId == itemId);

                if (item.RowId == 0)
                    continue;

                DisplayItem display = new()
                {
                    ItemId = item.RowId,
                    Name = item.Name.ExtractText(),
                    Level = item.LevelEquip,
                    IconId = item.Icon,
                    IsCraftable = false,
                    IsQuestReward = false,
                    IsDuty = false,
                    IsVendor = false,
                };

                slotItems.Add(display);
            }

            DisplayData[slot] = slotItems;
        }

        cacheThemeWindowSize = null;
        cacheScrollableHeight = null;
        cacheItems = null;
        uSlot = null;
        uTheme = null;

        LOG.Debug(
            $"Refreshed display data. Total themes populated: {DisplayData.Count}.");
    }

    private static float MeasureColumnWidth(IEnumerable<string?> values, string header, float extraPadding = 6f)
    {
        float width = ImGui.CalcTextSize(header).X;

        foreach (string? value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
                continue;

            width = Math.Max(width, ImGui.CalcTextSize(value).X);
        }

        return width
            + (ImGui.GetStyle().CellPadding.X * 2)
            + extraPadding;
    }

    internal class DisplayItem
    {
        public uint ItemId;
        public string Name = string.Empty;
        public uint Level;
        public uint IconId;
        public bool IsQuestReward = false;
        public bool IsDuty = false;
        public bool IsVendor = false;
        public bool IsCraftable = false;
        public bool IsSeals = false;
        public bool IsWolf = false;
    }
}