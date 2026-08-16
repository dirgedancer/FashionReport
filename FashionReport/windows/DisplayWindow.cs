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
    private int? openSolutionScore = null;
    internal Dictionary<string, List<DisplayItem>> DisplayData = new();
    public DisplayWindow() : base("Fashion Report Display Window", ImGuiWindowFlags.NoResize)
    {
        Size = _windowSize;
        SizeCondition = ImGuiCond.Always;
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

        Vector2 reportWindowPos = ImGui.GetWindowPos();
        float reportWindowWidth = ImGui.GetWindowWidth();

        DrawWeeklyHeader();
        DrawTopButtons();

        (float TableWidth, float TableHeight) = GeneralData.IsLongDisplay ? DrawLongTable() : DrawShortTable();

        Vector2 WindowPadding = ImGui.GetStyle().WindowPadding;
        const float tableTop = 130f;

        _windowSize = new Vector2(
            TableWidth + (WindowPadding.X * 2),
            tableTop + TableHeight + (WindowPadding.Y * 2) + 8f);        
            
        if (uSlot != null)
        {
            DrawThemeWindow(
                reportWindowPos,
                reportWindowWidth);
        }

        if (openSolutionScore != null)
        {
            DrawSolutionWindow(
                openSolutionScore.Value,
                reportWindowPos,
                reportWindowWidth);
        }
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
        FashionReportXivState? state =
            FashionReportXivProvider.CurrentState;

        ImGui.SetCursorPos(new Vector2(10f, 105f));

        string viewButtonText =
            GeneralData.IsLongDisplay ? "Short" : "Long";

        if (ImGui.Button(viewButtonText))
            GeneralData.IsLongDisplay = !GeneralData.IsLongDisplay;

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(
                GeneralData.IsLongDisplay
                    ? "Switch to Short View"
                    : "Switch to Long View");
        }

        float spacing = ImGui.GetStyle().ItemSpacing.X;

        ImGui.SameLine(0f, spacing);

        bool easy80Available =
            state?.Easy80Fresh == true &&
            state.Easy80 != null;

        if (!easy80Available)
            ImGui.BeginDisabled();

        if (ImGui.Button("Easy 80"))
        {
            bool closing = openSolutionScore == 80;

            openSolutionScore =
                closing ? null : 80;

            if (!closing)
            {
                uSlot = null;
                uTheme = null;
            }
        }

        if (!easy80Available)
            ImGui.EndDisabled();

        ImGui.SameLine(0f, spacing);

        bool easy100Available =
            state?.Easy100Fresh == true &&
            state.Easy100 != null;

        if (!easy100Available)
            ImGui.BeginDisabled();

        if (ImGui.Button("Easy 100"))
        {
            bool closing = openSolutionScore == 100;

            openSolutionScore =
                closing ? null : 100;

            if (!closing)
            {
                uSlot = null;
                uTheme = null;
            }
        }

        if (!easy100Available)
            ImGui.EndDisabled();
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
            if (uSlot == slot)
            {
                uSlot = null;
                uTheme = null;
            }
            else
            {
                uSlot = slot;
                uTheme = theme;

                // Only one attached companion panel at a time.
                openSolutionScore = null;
            }
        }
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        uint blueColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0.5f, 1f, 1f));
        Vector2 pos = ImGui.GetItemRectMin();
        drawList.AddText(pos, blueColor, theme);
        drawList.AddLine(new Vector2(pos.X, pos.Y + textSize.Y + 1), new Vector2(pos.X + textSize.X, pos.Y + textSize.Y + 1), blueColor, 1f);
    }

    private void DrawThemeWindow(
        Vector2 reportWindowPos,
        float reportWindowWidth)
    {
        if (uSlot == null || uTheme == null)
            return;

        if (!DisplayData.TryGetValue(
                uSlot,
                out List<DisplayItem>? items))
        {
            return;
        }

        const float iconSize = 32f;
        const float rowHeight = 40f;
        const float attachmentGap = 4f;
        const int maxVisibleRows = 8;

        float maxItemNameWidth = items.Count > 0
            ? items.Max(item => ImGui.CalcTextSize(item.Name).X)
            : 0f;

        float titleWidth = ImGui.CalcTextSize(uTheme).X;

        float panelWidth = Math.Clamp(
            Math.Max(
                titleWidth + 60f,
                maxItemNameWidth + iconSize + 70f),
            280f,
            460f);

        ImGuiViewportPtr viewport = ImGui.GetMainViewport();

        float panelX =
            reportWindowPos.X
            - panelWidth
            - attachmentGap;

        // Fall back to the right side if there isn't enough
        // screen space to attach on the left.
        if (panelX < viewport.WorkPos.X)
        {
            panelX =
                reportWindowPos.X
                + reportWindowWidth
                + attachmentGap;
        }

        ImGui.SetNextWindowPos(
            new Vector2(panelX, reportWindowPos.Y),
            ImGuiCond.Always);

        ImGui.SetNextWindowSizeConstraints(
            new Vector2(panelWidth, 0f),
            new Vector2(panelWidth, float.MaxValue));

        bool open = true;

        if (ImGui.Begin(
            $"{uTheme}###FashionReportThemeItems",
            ref open,
            ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoSavedSettings))
        {
            if (items.Count == 0)
            {
                ImGui.TextDisabled(
                    "No qualifying items are available.");
            }
            else
            {
                int visibleRows =
                    Math.Min(items.Count, maxVisibleRows);

                float childHeight =
                    visibleRows * rowHeight;

                ImGui.BeginChild(
                    "ThemeItemScrollRegion",
                    new Vector2(0f, childHeight),
                    false);

                if (ImGui.BeginTable(
                    "ThemeItemTable",
                    2,
                    ImGuiTableFlags.SizingFixedFit
                        | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn(
                        "Icon",
                        ImGuiTableColumnFlags.WidthFixed,
                        iconSize + 6f);

                    ImGui.TableSetupColumn(
                        "Item",
                        ImGuiTableColumnFlags.WidthStretch);

                    foreach (DisplayItem item in items)
                    {
                        ImGui.TableNextRow(
                            ImGuiTableRowFlags.None,
                            rowHeight);

                        ImGui.TableSetColumnIndex(0);

                        if (SERVICES.Texture
                            .GetFromGameIcon(
                                new GameIconLookup
                                {
                                    IconId = item.IconId
                                })
                            .TryGetWrap(
                                out IDalamudTextureWrap? icon,
                                out _)
                            && icon != null)
                        {
                            ImGui.Image(
                                icon.Handle,
                                new Vector2(iconSize, iconSize));
                        }

                        ImGui.TableSetColumnIndex(1);

                        ImGui.TextUnformatted(item.Name);

                        ImGui.TextDisabled(
                            $"Level {item.Level}");
                    }

                    ImGui.EndTable();
                }

                ImGui.EndChild();
            }
        }

        ImGui.End();

        if (!open)
        {
            uSlot = null;
            uTheme = null;
        }
    }

    private Vector4 GetIconColor(uint iconId) => IconColors[(iconId - 22804) % 9];

    public void Dispose()
    {
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

        uSlot = null;
        uTheme = null;

        LOG.Debug(
            $"Refreshed display data. Total themes populated: {DisplayData.Count}.");
    }

    private static string FormatSolutionSlot(string slot)
    {
        return slot.ToLowerInvariant() switch
        {
            "weapon" => "Weapon",
            "head" => "Head",
            "body" => "Body",
            "hands" => "Gloves",
            "gloves" => "Gloves",
            "legs" => "Legs",
            "feet" => "Boots",
            "boots" => "Boots",
            "earrings" => "Earrings",
            "neck" => "Necklace",
            "necklace" => "Necklace",
            "wrist" => "Bracelet",
            "bracelet" => "Bracelet",
            "ring" => "Ring",
            _ => slot
        };
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

    private void DrawSolutionButtons()
    {
        FashionReportXivState? state =
            FashionReportXivProvider.CurrentState;

        if (state == null)
            return;

        ImGui.SetCursorPos(new Vector2(75f, 105f));

        bool easy80Available =
            state.Easy80Fresh &&
            state.Easy80 != null;

        if (!easy80Available)
            ImGui.BeginDisabled();

        if (ImGui.Button("Easy 80"))
            openSolutionScore = 80;

        if (!easy80Available)
            ImGui.EndDisabled();

        ImGui.SameLine();

        bool easy100Available =
            state.Easy100Fresh &&
            state.Easy100 != null;

        if (!easy100Available)
            ImGui.BeginDisabled();

        if (ImGui.Button("Easy 100"))
            openSolutionScore = 100;

        if (!easy100Available)
            ImGui.EndDisabled();
    }

        private void DrawSolutionWindow(
            int score,
            Vector2 reportWindowPos,
            float reportWindowWidth)
        {
        FashionReportXivState? state =
            FashionReportXivProvider.CurrentState;

        FashionReportXivSolution? solution =
            score == 80 ? state?.Easy80 : state?.Easy100;

        if (solution == null)
        {
            openSolutionScore = null;
            return;
        }

        bool open = true;

        const float solutionWidth = 220f;
        const float attachmentGap = 4f;

        ImGuiViewportPtr viewport = ImGui.GetMainViewport();

        float solutionX =
            reportWindowPos.X - solutionWidth - attachmentGap;

        // If there isn't enough room on the left, attach it
        // to the right instead of letting it go off-screen.
        if (solutionX < viewport.WorkPos.X)
        {
            solutionX =
                reportWindowPos.X
                + reportWindowWidth
                + attachmentGap;
        }

        Vector2 solutionPos = new(
            solutionX,
            reportWindowPos.Y);

        ImGui.SetNextWindowPos(
            solutionPos,
            ImGuiCond.Always);

        ImGui.SetNextWindowSizeConstraints(
            new Vector2(solutionWidth, 0f),
            new Vector2(solutionWidth, float.MaxValue));

        if (ImGui.Begin(
            $"Easy {score} Solution###FashionReportEasy{score}",
            ref open,
            ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoSavedSettings))
        {
            ImGui.Text($"FashionReportXIV Easy {score}");
            ImGui.Separator();

            if (solution.ItemPairs.Count > 0)
            {
                ImGui.Text("Gear");

                foreach (FashionReportXivItemPair item in solution.ItemPairs)
                {
                    ImGui.BulletText(
                        $"{FormatSolutionSlot(item.Slot)}: {item.Name}");
                }
            }

            IEnumerable<KeyValuePair<string, string>> dyes =
                solution.Dyes.Where(x =>
                    !string.IsNullOrWhiteSpace(x.Value));

            if (dyes.Any())
            {
                if (solution.ItemPairs.Count > 0)
                    ImGui.Spacing();

                ImGui.Text("Dyes");

                foreach ((string slot, string dye) in dyes)
                {
                    ImGui.BulletText(
                        $"{FormatSolutionSlot(slot)}: {dye}");
                }
            }
        }

        ImGui.End();

        if (!open)
            openSolutionScore = null;
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