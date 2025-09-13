using Dalamud.Configuration;
using FashionReport;

[Serializable]
public class FashionReportDataStorage : IPluginConfiguration
{
    public int Version { get; set; } = 1;
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
        if (MySql.IsWeekUpdated(Week)) { LOG.Info($"FashionReport week {Week} already updated."); return; }
        await MySql.InsertFashionReport(Week, WeeklyTheme, Weapon, Head, Body, Gloves, Legs, Boots, Earrings, Necklace, Bracelet, RightRing, LeftRing, Timestamp, WeaponDye, HeadDye, BodyDye, GlovesDye, LegsDye, BootsDye);
        LOG.Info($"FashionReport week {Week} has been updated to the database.");
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

        Weapon = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == WeaponThemeName).RowId;
        Head = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == HeadThemeName).RowId;
        Body = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == BodyThemeName).RowId;
        Gloves = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == GlovesThemeName).RowId;
        Legs = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == LegsThemeName).RowId;
        Boots = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == BootsThemeName).RowId;
        Earrings = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == EarringsThemeName).RowId;
        Necklace = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == NecklaceThemeName).RowId;
        Bracelet = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == BraceletThemeName).RowId;
        RightRing = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == RightRingThemeName).RowId;
        LeftRing = SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.Name.ToString() == LeftRingThemeName).RowId;


        WeaponDye = fashionCheck.WeaponShade1 != 0 ? fashionCheck.WeaponShade1 : null;
        HeadDye = fashionCheck.HeadShade1 != 0 ? fashionCheck.HeadShade1 : null;
        BodyDye = fashionCheck.BodyShade1 != 0 ? fashionCheck.BodyShade1 : null;
        GlovesDye = fashionCheck.HandsShade1 != 0 ? fashionCheck.HandsShade1 : null;
        LegsDye = fashionCheck.LegsShade1 != 0 ? fashionCheck.LegsShade1 : null;
        BootsDye = fashionCheck.FeetShade1 != 0 ? fashionCheck.FeetShade1 : null;

        WeaponDyeName = WeaponDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == WeaponDye).Name.ToString() : string.Empty;
        HeadDyeName = HeadDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == HeadDye).Name.ToString() : string.Empty;
        BodyDyeName = BodyDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == BodyDye).Name.ToString() : string.Empty;
        GlovesDyeName = GlovesDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == GlovesDye).Name.ToString() : string.Empty;
        LegsDyeName = LegsDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == LegsDye).Name.ToString() : string.Empty;
        BootsDyeName = BootsDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == BootsDye).Name.ToString() : string.Empty;

        WeaponData = Weapon != null ? MySql.GetItemsForSlot(Weapon.Value, 1) : null;
        HeadData = Head != null ? MySql.GetItemsForSlot(Head.Value, 3) : null;
        BodyData = Body != null ? MySql.GetItemsForSlot(Body.Value, 4) : null;
        GlovesData = Gloves != null ? MySql.GetItemsForSlot(Gloves.Value, 5) : null;
        LegsData = Legs != null ? MySql.GetItemsForSlot(Legs.Value, 7) : null;
        BootsData = Boots != null ? MySql.GetItemsForSlot(Boots.Value, 8) : null;
        EarringsData = Earrings != null ? MySql.GetItemsForSlot(Earrings.Value, 9) : null;
        NecklaceData = Necklace != null ? MySql.GetItemsForSlot(Necklace.Value, 10) : null;
        BraceletData = Bracelet != null ? MySql.GetItemsForSlot(Bracelet.Value, 11) : null;
        RightRingData = RightRing != null ? MySql.GetItemsForSlot(RightRing.Value, 12) : null;
        LeftRingData = LeftRing != null ? MySql.GetItemsForSlot(LeftRing.Value, 12) : null;

        Timestamp = (ulong)new DateTimeOffset(FashionReportPoller.GetFridayOfDyeWeek(Week)).ToUnixTimeSeconds();
    }

    public static FashionReportDataStorage LoadData() => SERVICES.Interface.GetPluginConfig() as FashionReportDataStorage ?? new FashionReportDataStorage();
    public void SaveData() => SERVICES.Interface.SavePluginConfig(this);
}
