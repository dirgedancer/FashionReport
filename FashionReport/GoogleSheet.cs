using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using Dalamud.Game.ClientState.Objects.SubKinds;
using System.Globalization;
using System.Runtime.CompilerServices;
using Lumina.Excel;
using Lumina.Excel.Sheets;


namespace FashionReportCalculator;

internal static class GoogleSheetData
{
    private static SheetsService? _service;

    internal static void Initialize()
    {
        if (_service != null) return;
        string? credentialsJson = Environment.GetEnvironmentVariable("GOOGLESHEETSREADONLYKEY");
        if (string.IsNullOrEmpty(credentialsJson))
            throw new InvalidOperationException("GOOGLESHEETSREADONLYKEY environment variable is not set.");
        GoogleCredential credential = GoogleCredential.FromJson(credentialsJson).CreateScoped(SheetsService.Scope.Spreadsheets);
        _service = new SheetsService(new BaseClientService.Initializer { HttpClientInitializer = credential, ApplicationName = "FashionReportPlugin" });
    }

    private static async Task<List<List<uint>>> FetchSheet(string sheetName)
    {
        if (_service == null) Initialize();
        try
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = _service!.Spreadsheets.Values.Get("1RWNR3MeKq49wfGVEBGIhDMtrJL40uhbtzuZtIpUbVw8", sheetName);
            ValueRange response = await request.ExecuteAsync();
            List<List<uint>> data = new();
            if (response.Values != null)
                foreach (IList<object>? row in response.Values)
                {
                    List<uint> rowData = new();
                    foreach (object? cell in row)
                        rowData.Add(uint.TryParse(cell?.ToString() ?? "0", out uint v) ? v : 0);
                    data.Add(rowData);
                }
            return data;
        }
        catch (Google.Apis.Auth.OAuth2.Responses.TokenResponseException ex)
        {
            if (ex.Error != null && ex.Error.Error == "invalid_grant")
            {
                Initialize();
                return await FetchSheet(sheetName);
            }
            throw;
        }
    }


    internal static async Task<bool> WeekExists(uint week)
    {
        List<List<uint>> data = await FetchSheet("Data");
        return data.Any(r => r.Count > 0 && r[0] == week);
    }

    internal static async Task<bool> IsWeekUpdated(uint week)
    {
        List<List<uint>> data = await FetchSheet("Data");
        return data.Any(r => r.Count > 0 && r[0] == week);
    }

    internal static async Task<List<uint>> GetItemsForSlot(uint themeId, int slotId)
    {
        List<List<uint>> data = await FetchSheet("Data");
        return data.Where(r => r.Count > 2 && r[0] == themeId && r[2] == slotId).Select(r => r[1]).ToList();
    }

    internal static async Task<List<uint>> GetThemeItemsForSlot(uint themeId, int slotId)
    {
        List<List<uint>> themes = await FetchSheet("Theme");
        return themes.Where(r => r.Count > 2 && r[0] == themeId && r[2] == slotId).Select(r => r[1]).ToList();
    }

    internal static async Task<FashionReportDataStorage?> GetLatestReport()
    {
        List<List<uint>> data = await FetchSheet("Data");
        if (data.Count == 0) return null;
        int x = data.Count - 1;
        FashionReportDataStorage temp = new FashionReportDataStorage
        {
            Week = data[x][0],
            WeeklyTheme = (uint)data[x][1],
            Weapon = (uint?)data[x][2],
            Head = (uint?)data[x][3],
            Body = (uint?)data[x][4],
            Gloves = (uint?)data[x][5],
            Legs = (uint?)data[x][6],
            Boots = (uint?)data[x][7],
            Earrings = (uint?)data[x][8],
            Necklace = (uint?)data[x][9],
            Bracelet = (uint?)data[x][10],
            RightRing = (uint?)data[x][11],
            LeftRing = (uint?)data[x][12],
            WeaponDye = (uint?)data[x][14],
            HeadDye = (uint?)data[x][15],
            BodyDye = (uint?)data[x][16],
            GlovesDye = (uint?)data[x][17],
            LegsDye = (uint?)data[x][18],
            BootsDye = (uint?)data[x][19]
        };
        await temp.ProcessFromId();
        return temp;
    }
}

internal static class GoogleSheetWriter
{
    private static readonly HttpClient _httpClient = new();
    private const string ProxyBaseUrl = "https://ScarBot.ddns.net/FashionReport";

    internal static async Task InsertFashionReport(uint week, uint? weeklyTheme, uint? weapon, uint? head, uint? body, uint? gloves, uint? legs, uint? boots, uint? earrings, uint? necklace, uint? bracelet, uint? rightRing, uint? leftRing, ulong date, uint? weaponDye, uint? headDye, uint? bodyDye, uint? glovesDye, uint? legsDye, uint? bootsDye)
    {
        IPlayerCharacter? Local = null;
        try { await SERVICES.Framework.RunOnTick(() => Local = SERVICES.ClientState.LocalPlayer); }
        catch (Exception ex) { LOG.Debug($"LocalPlayer crash: {ex}"); return; }
        if (Local == null) return;
        string name = Local.Name.ToString();
        string[] parts = name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        string firstName = parts.Length > 0 ? parts[0] : string.Empty;
        string lastName = parts.Length > 1 ? parts[1] : string.Empty;
        ExcelSheet<World>? worlds = SERVICES.Data.GetExcelSheet<World>();
        World? homeWorld = worlds?.FirstOrDefault(w => w.RowId == Local.HomeWorld.RowId);
        string server = homeWorld?.Name.ToString() ?? "Unknown";
        using MemoryStream ms = new();
        using BinaryWriter bw = new(ms, Encoding.UTF8, true);
        WriteString(bw, DiscordOAuth._discordConfiguration.Discord);
        WriteString(bw, firstName);
        WriteString(bw, lastName);
        WriteString(bw, server);
        bw.Write(week);
        bw.Write(weeklyTheme ?? 0);
        bw.Write(weapon ?? 0);
        bw.Write(head ?? 0);
        bw.Write(body ?? 0);
        bw.Write(gloves ?? 0);
        bw.Write(legs ?? 0);
        bw.Write(boots ?? 0);
        bw.Write(earrings ?? 0);
        bw.Write(necklace ?? 0);
        bw.Write(bracelet ?? 0);
        bw.Write(rightRing ?? 0);
        bw.Write(leftRing ?? 0);
        bw.Write(date);
        bw.Write(weaponDye ?? 0);
        bw.Write(headDye ?? 0);
        bw.Write(bodyDye ?? 0);
        bw.Write(glovesDye ?? 0);
        bw.Write(legsDye ?? 0);
        bw.Write(bootsDye ?? 0);
        await PostBinary(ms.ToArray(), "Insert");
    }

    internal static async Task InsertFashionReportDye(DyeStruct dye)
    {
        string token = DiscordOAuth._discordConfiguration.Discord;
        if (string.IsNullOrEmpty(token)) return;
        IPlayerCharacter? player = SERVICES.ClientState.LocalPlayer;
        if (player == null) return;
        string name = player.Name.ToString();
        string[] parts = name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        string firstName = parts.Length > 0 ? parts[0] : string.Empty;
        string lastName = parts.Length > 1 ? parts[1] : string.Empty;
        string server = player.HomeWorld.ToString()!;
        using MemoryStream ms = new();
        using BinaryWriter bw = new(ms, Encoding.UTF8, true);
        WriteString(bw, token);
        WriteString(bw, firstName);
        WriteString(bw, lastName);
        WriteString(bw, server);
        bw.Write(dye.Score);
        bw.Write(dye.WeaponItemId);
        bw.Write(dye.WeaponGlamourId);
        bw.Write(dye.WeaponDye1 ?? 0);
        bw.Write(dye.WeaponDye2 ?? 0);
        bw.Write(dye.WeaponTheme);
        bw.Write(dye.WeaponPicture);
        bw.Write(dye.WeaponPictureInfo);
        bw.Write(dye.HeadItemId);
        bw.Write(dye.HeadGlamourId);
        bw.Write(dye.HeadDye1 ?? 0);
        bw.Write(dye.HeadDye2 ?? 0);
        bw.Write(dye.HeadTheme);
        bw.Write(dye.HeadPicture);
        bw.Write(dye.HeadPictureInfo);
        bw.Write(dye.BodyItemId);
        bw.Write(dye.BodyGlamourId);
        bw.Write(dye.BodyDye1 ?? 0);
        bw.Write(dye.BodyDye2 ?? 0);
        bw.Write(dye.BodyTheme);
        bw.Write(dye.BodyPicture);
        bw.Write(dye.BodyPictureInfo);
        bw.Write(dye.GlovesItemId);
        bw.Write(dye.GlovesGlamourId);
        bw.Write(dye.GlovesDye1 ?? 0);
        bw.Write(dye.GlovesDye2 ?? 0);
        bw.Write(dye.GlovesTheme);
        bw.Write(dye.GlovesPicture);
        bw.Write(dye.GlovesPictureInfo);
        bw.Write(dye.LegsItemId);
        bw.Write(dye.LegsGlamourId);
        bw.Write(dye.LegsDye1 ?? 0);
        bw.Write(dye.LegsDye2 ?? 0);
        bw.Write(dye.LegsTheme);
        bw.Write(dye.LegsPicture);
        bw.Write(dye.LegsPictureInfo);
        bw.Write(dye.BootsItemId);
        bw.Write(dye.BootsGlamourId);
        bw.Write(dye.BootsDye1 ?? 0);
        bw.Write(dye.BootsDye2 ?? 0);
        bw.Write(dye.BootsTheme);
        bw.Write(dye.BootsPicture);
        bw.Write(dye.BootsPictureInfo);
        bw.Write(dye.EarringsItemId);
        bw.Write(dye.EarringsGlamourId);
        bw.Write(dye.EarringsTheme);
        bw.Write(dye.EarringsPicture);
        bw.Write(dye.EarringsPictureInfo);
        bw.Write(dye.NecklaceItemId);
        bw.Write(dye.NecklaceGlamourId);
        bw.Write(dye.NecklaceTheme);
        bw.Write(dye.NecklacePicture);
        bw.Write(dye.NecklacePictureInfo);
        bw.Write(dye.BraceletItemId);
        bw.Write(dye.BraceletGlamourId);
        bw.Write(dye.BraceletTheme);
        bw.Write(dye.BraceletPicture);
        bw.Write(dye.BraceletPictureInfo);
        bw.Write(dye.RightRingItemId);
        bw.Write(dye.RightRingGlamourId);
        bw.Write(dye.RightRingTheme);
        bw.Write(dye.RightRingPicture);
        bw.Write(dye.RightRingPictureInfo);
        bw.Write(dye.LeftRingItemId);
        bw.Write(dye.LeftRingGlamourId);
        bw.Write(dye.LeftRingTheme);
        bw.Write(dye.LeftRingPicture);
        bw.Write(dye.LeftRingPictureInfo);
        await PostBinary(ms.ToArray(), "Dye");
    }

    private static void WriteString(BinaryWriter bw, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        bw.Write((ushort)bytes.Length);
        bw.Write(bytes);
    }

    private static async Task PostBinary(byte[] data, string endpoint)
    {
        try
        {
            using ByteArrayContent content = new(data);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            HttpResponseMessage response = await _httpClient.PostAsync($"{ProxyBaseUrl}{endpoint}", content);
            if (!response.IsSuccessStatusCode)
                LOG.Error($"POST {endpoint} failed ({response.StatusCode})");
        }
        catch (Exception ex) { LOG.Error($"POST {endpoint} exception: {ex.Message}"); }
    }
}