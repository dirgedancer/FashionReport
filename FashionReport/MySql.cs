using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FashionReport;

internal static class MySql
{
    internal static MySqlConnection GetConnection()
    {
        string connectionString = NewIcon.GetIcon();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            LOG.Error("MySql.GetConnection called but connection string is not configured");
            throw new InvalidOperationException("Connection string is not configured.");
        }
        return new MySqlConnection(connectionString);
    }

    internal static DateTime? GetLastUpdate(uint week)
    {
        try
        {
            using MySqlConnection conn = GetConnection();
            conn.Open();
            using MySqlCommand cmd = new("SELECT last_update FROM fashionreport_status WHERE week = @week", conn);
            cmd.Parameters.AddWithValue("@week", week);
            object? result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDateTime(result) : null;
        }
        catch (Exception ex) { LOG.Error($"MySql.GetLastUpdate: {ex.Message}"); return null; }
    }

    internal static void InsertOrUpdateStatus(uint week, DateTime lastUpdate, string reportType)
    {
        try
        {
            using MySqlConnection conn = GetConnection();
            conn.Open();
            using MySqlCommand cmd = new(
                @"INSERT INTO fashionreport_status (week, last_update, report_type) 
                      VALUES (@week, @lastUpdate, @reportType)
                      ON DUPLICATE KEY UPDATE last_update = @lastUpdate, report_type = @reportType;", conn);
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@lastUpdate", lastUpdate);
            cmd.Parameters.AddWithValue("@reportType", reportType);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { LOG.Error($"MySql.InsertOrUpdateStatus: {ex.Message}"); }
    }

    internal static async Task InsertFashionReport(uint week, uint? weeklyTheme, uint? weapon, uint? head, uint? body, uint? gloves, uint? legs, uint? boots, uint? earrings, uint? necklace, uint? bracelet, uint? rightRing, uint? leftRing, ulong date, uint? weaponDye, uint? headDye, uint? bodyDye, uint? glovesDye, uint? legsDye, uint? bootsDye)
    {
        try
        {
            using TcpClient client = new("scarbot.ddns.net", 6000);
            using NetworkStream stream = client.GetStream();
            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);
            writer.Write((byte)1);
            writer.Write(week);
            writer.Write(weeklyTheme ?? (uint)0);
            writer.Write(weapon ?? (uint)0);
            writer.Write(head ?? (uint)0);
            writer.Write(body ?? (uint)0);
            writer.Write(gloves ?? (uint)0);
            writer.Write(legs ?? (uint)0);
            writer.Write(boots ?? (uint)0);
            writer.Write(earrings ?? (uint)0);
            writer.Write(necklace ?? (uint)0);
            writer.Write(bracelet ?? (uint)0);
            writer.Write(rightRing ?? (uint)0);
            writer.Write(leftRing ?? (uint)0);
            writer.Write((ulong)new DateTimeOffset(2018, 1, 26, 8, 0, 0, TimeSpan.Zero).AddDays(week * 7).ToUnixTimeSeconds());
            writer.Write(weaponDye ?? (uint)0);
            writer.Write(headDye ?? (uint)0);
            writer.Write(bodyDye ?? (uint)0);
            writer.Write(glovesDye ?? (uint)0);
            writer.Write(legsDye ?? (uint)0);
            writer.Write(bootsDye ?? (uint)0);
            await stream.WriteAsync(ms.ToArray(), 0, (int)ms.Length);
        }
        catch (Exception ex) { LOG.Error($"FashionReportClient.InsertFashionReport: {ex.Message}"); }
    }

    internal static async Task InsertFashionReportDye(DyeStruct dye)
    {
        try
        {
            using TcpClient client = new("scarbot.ddns.net", 6000);
            using NetworkStream stream = client.GetStream();
            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);
            writer.Write((byte)2);
            writer.Write(dye.Score);
            writer.Write(dye.WeaponItemId);
            writer.Write(dye.WeaponGlamourId);
            writer.Write(dye.WeaponDye1 ?? (uint)0);
            writer.Write(dye.WeaponDye2 ?? (uint)0);
            writer.Write(dye.WeaponTheme);
            writer.Write(dye.WeaponPicture);
            writer.Write(dye.WeaponPictureInfo);
            writer.Write(dye.HeadItemId);
            writer.Write(dye.HeadGlamourId);
            writer.Write(dye.HeadDye1 ?? (uint)0);
            writer.Write(dye.HeadDye2 ?? (uint)0);
            writer.Write(dye.HeadTheme);
            writer.Write(dye.HeadPicture);
            writer.Write(dye.HeadPictureInfo);
            writer.Write(dye.BodyItemId);
            writer.Write(dye.BodyGlamourId);
            writer.Write(dye.BodyDye1 ?? (uint)0);
            writer.Write(dye.BodyDye2 ?? (uint)0);
            writer.Write(dye.BodyTheme);
            writer.Write(dye.BodyPicture);
            writer.Write(dye.BodyPictureInfo);
            writer.Write(dye.GlovesItemId);
            writer.Write(dye.GlovesGlamourId);
            writer.Write(dye.GlovesDye1 ?? (uint)0);
            writer.Write(dye.GlovesDye2 ?? (uint)0);
            writer.Write(dye.GlovesTheme);
            writer.Write(dye.GlovesPicture);
            writer.Write(dye.GlovesPictureInfo);
            writer.Write(dye.LegsItemId);
            writer.Write(dye.LegsGlamourId);
            writer.Write(dye.LegsDye1 ?? (uint)0);
            writer.Write(dye.LegsDye2 ?? (uint)0);
            writer.Write(dye.LegsTheme);
            writer.Write(dye.LegsPicture);
            writer.Write(dye.LegsPictureInfo);
            writer.Write(dye.BootsItemId);
            writer.Write(dye.BootsGlamourId);
            writer.Write(dye.BootsDye1 ?? (uint)0);
            writer.Write(dye.BootsDye2 ?? (uint)0);
            writer.Write(dye.BootsTheme);
            writer.Write(dye.BootsPicture);
            writer.Write(dye.BootsPictureInfo);
            writer.Write(dye.EarringsItemId);
            writer.Write(dye.EarringsGlamourId);
            writer.Write(dye.EarringsTheme);
            writer.Write(dye.EarringsPicture);
            writer.Write(dye.EarringsPictureInfo);
            writer.Write(dye.NecklaceItemId);
            writer.Write(dye.NecklaceGlamourId);
            writer.Write(dye.NecklaceTheme);
            writer.Write(dye.NecklacePicture);
            writer.Write(dye.NecklacePictureInfo);
            writer.Write(dye.BraceletItemId);
            writer.Write(dye.BraceletGlamourId);
            writer.Write(dye.BraceletTheme);
            writer.Write(dye.BraceletPicture);
            writer.Write(dye.BraceletPictureInfo);
            writer.Write(dye.RightRingItemId);
            writer.Write(dye.RightRingGlamourId);
            writer.Write(dye.RightRingTheme);
            writer.Write(dye.RightRingPicture);
            writer.Write(dye.RightRingPictureInfo);
            writer.Write(dye.LeftRingItemId);
            writer.Write(dye.LeftRingGlamourId);
            writer.Write(dye.LeftRingTheme);
            writer.Write(dye.LeftRingPicture);
            writer.Write(dye.LeftRingPictureInfo);
            await stream.WriteAsync(ms.ToArray(), 0, (int)ms.Length);
        }
        catch (Exception ex) { LOG.Error($"FashionReportClient.InsertFashionReportDye: {ex.Message}"); }
    }

    internal static bool IsWeekUpdated(uint week)
    {
        try
        {
            using MySqlConnection conn = GetConnection();
            conn.Open();
            using MySqlCommand cmd = new("SELECT 1 FROM fashionreport_status WHERE week = @week LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@week", week);
            return cmd.ExecuteScalar() != null;
        }
        catch (Exception ex) { LOG.Error($"MySql.IsWeekUpdated: {ex.Message}"); return false; }
    }

    internal static bool WeekExists(uint week)
    {
        using MySqlConnection conn = GetConnection();
        conn.Open();
        using MySqlCommand cmd = new("SELECT COUNT(*) FROM fashionreportdata WHERE Week = @week", conn);
        cmd.Parameters.AddWithValue("@week", week);
        return (ulong)cmd.ExecuteScalar()! > 0;
    }

    internal static int GetDiscoveredDyeCount(uint week)
    {
        using MySqlConnection conn = GetConnection();
        conn.Open();
        using MySqlCommand cmd = new("SELECT COUNT(DISTINCT dye_index) FROM fashionreport_dye WHERE Week = @week", conn);
        cmd.Parameters.AddWithValue("@week", week);
        return Convert.ToInt32(cmd.ExecuteScalar()!);
    }

    internal static List<uint> GetItemsForSlot(uint themeId, int slotId)
    {
        List<uint> items = new();
        try
        {
            using MySqlConnection conn = GetConnection();
            conn.Open();
            using MySqlCommand cmd = new("SELECT ItemId FROM ThemeItems WHERE ThemeId = @themeId AND SlotId = @slotId", conn);
            cmd.Parameters.AddWithValue("@themeId", themeId);
            cmd.Parameters.AddWithValue("@slotId", slotId);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                items.Add(Convert.ToUInt32(reader["ItemId"]));
        }
        catch (Exception ex) { LOG.Error($"MySql.GetItemsForSlot: {ex.Message}"); }
        return items;
    }

    internal static FashionReportDataStorage? GetLatestReport()
    {
        try
        {
            using MySqlConnection conn = GetConnection();
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT * FROM fashionreportdata ORDER BY Week DESC LIMIT 1", conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            FashionReportDataStorage data = new FashionReportDataStorage
            {
                Week = Convert.ToUInt32(reader["Week"]),
                WeeklyTheme = Convert.ToUInt32(reader["WeeklyTheme"]),
                Weapon = reader["Weapon"] != DBNull.Value ? Convert.ToUInt32(reader["Weapon"]) : null,
                Head = reader["Head"] != DBNull.Value ? Convert.ToUInt32(reader["Head"]) : null,
                Body = reader["Body"] != DBNull.Value ? Convert.ToUInt32(reader["Body"]) : null,
                Gloves = reader["Gloves"] != DBNull.Value ? Convert.ToUInt32(reader["Gloves"]) : null,
                Legs = reader["Legs"] != DBNull.Value ? Convert.ToUInt32(reader["Legs"]) : null,
                Boots = reader["Boots"] != DBNull.Value ? Convert.ToUInt32(reader["Boots"]) : null,
                Earrings = reader["Earrings"] != DBNull.Value ? Convert.ToUInt32(reader["Earrings"]) : null,
                Necklace = reader["Necklace"] != DBNull.Value ? Convert.ToUInt32(reader["Necklace"]) : null,
                Bracelet = reader["Bracelet"] != DBNull.Value ? Convert.ToUInt32(reader["Bracelet"]) : null,
                RightRing = reader["RightRing"] != DBNull.Value ? Convert.ToUInt32(reader["RightRing"]) : null,
                LeftRing = reader["LeftRing"] != DBNull.Value ? Convert.ToUInt32(reader["LeftRing"]) : null,
                WeaponDye = reader["WeaponDye"] != DBNull.Value ? Convert.ToUInt32(reader["WeaponDye"]) : null,
                HeadDye = reader["HeadDye"] != DBNull.Value ? Convert.ToUInt32(reader["HeadDye"]) : null,
                BodyDye = reader["BodyDye"] != DBNull.Value ? Convert.ToUInt32(reader["BodyDye"]) : null,
                GlovesDye = reader["GlovesDye"] != DBNull.Value ? Convert.ToUInt32(reader["GlovesDye"]) : null,
                LegsDye = reader["LegsDye"] != DBNull.Value ? Convert.ToUInt32(reader["LegsDye"]) : null,
                BootsDye = reader["BootsDye"] != DBNull.Value ? Convert.ToUInt32(reader["BootsDye"]) : null
            };

            data.WeeklyThemeName = SERVICES.AllWeeklyFashionThemes.FirstOrDefault(x => x.RowId == data.WeeklyTheme).Name.ToString() ?? string.Empty;

            data.WeaponData = data.Weapon != null ? MySql.GetItemsForSlot(data.Weapon.Value, 1) : null;
            data.HeadData = data.Head != null ? MySql.GetItemsForSlot(data.Head.Value, 3) : null;
            data.BodyData = data.Body != null ? MySql.GetItemsForSlot(data.Body.Value, 4) : null;
            data.GlovesData = data.Gloves != null ? MySql.GetItemsForSlot(data.Gloves.Value, 5) : null;
            data.LegsData = data.Legs != null ? MySql.GetItemsForSlot(data.Legs.Value, 7) : null;
            data.BootsData = data.Boots != null ? MySql.GetItemsForSlot(data.Boots.Value, 8) : null;
            data.EarringsData = data.Earrings != null ? MySql.GetItemsForSlot(data.Earrings.Value, 9) : null;
            data.NecklaceData = data.Necklace != null ? MySql.GetItemsForSlot(data.Necklace.Value, 10) : null;
            data.BraceletData = data.Bracelet != null ? MySql.GetItemsForSlot(data.Bracelet.Value, 11) : null;
            data.RightRingData = data.RightRing != null ? MySql.GetItemsForSlot(data.RightRing.Value, 12) : null;
            data.LeftRingData = data.LeftRing != null ? MySql.GetItemsForSlot(data.LeftRing.Value, 12) : null;

            data.WeaponThemeName = data.Weapon != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Weapon.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.HeadThemeName = data.Head != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Head.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.BodyThemeName = data.Body != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Body.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.GlovesThemeName = data.Gloves != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Gloves.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.LegsThemeName = data.Legs != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Legs.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.BootsThemeName = data.Boots != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Boots.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.EarringsThemeName = data.Earrings != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Earrings.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.NecklaceThemeName = data.Necklace != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Necklace.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.BraceletThemeName = data.Bracelet != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.Bracelet.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.RightRingThemeName = data.RightRing != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.RightRing.Value).Name.ToString() ?? string.Empty : string.Empty;
            data.LeftRingThemeName = data.LeftRing != null ? SERVICES.AllFashionThemeCategories.FirstOrDefault(x => x.RowId == data.LeftRing.Value).Name.ToString() ?? string.Empty : string.Empty;

            data.WeaponDyeName = data.WeaponDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == data.WeaponDye).Name.ToString() ?? string.Empty : string.Empty;
            data.HeadDyeName = data.HeadDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == data.HeadDye).Name.ToString() ?? string.Empty : string.Empty;
            data.BodyDyeName = data.BodyDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == data.BodyDye).Name.ToString() ?? string.Empty : string.Empty;
            data.GlovesDyeName = data.GlovesDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == data.GlovesDye).Name.ToString() ?? string.Empty : string.Empty;
            data.LegsDyeName = data.LegsDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == data.LegsDye).Name.ToString() ?? string.Empty : string.Empty;
            data.BootsDyeName = data.BootsDye != null ? SERVICES.AllStains.FirstOrDefault(x => x.RowId == data.BootsDye).Name.ToString() ?? string.Empty : string.Empty;

            return data;
        }
        catch (Exception ex) { LOG.Error($"MySql.GetLatestReport: {ex.Message}"); return null; }
    }
}
