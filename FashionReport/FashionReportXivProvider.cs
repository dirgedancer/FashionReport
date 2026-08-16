using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FashionReportCalculator;

internal static class FashionReportXivProvider
{
    internal static FashionReportXivState? CurrentState { get; private set; }
    private const string ReportStateUrl =
        "https://fashionreportxiv.com/api/report-state";

    private static readonly HttpClient Http = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    internal static async Task<FashionReportXivState?> FetchAsync(
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response =
            await Http.GetAsync(ReportStateUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<FashionReportXivState>(
            JsonOptions,
            cancellationToken);
    }

    internal static async Task<FashionReportDataStorage?> GetCurrentReportAsync(
        CancellationToken cancellationToken = default)
    {
        FashionReportXivState? state = await FetchAsync(cancellationToken);

        if (state?.LastOptions == null)
            return null;

        if (!uint.TryParse(state.LastOptions.Week, out uint week))
        {
            LOG.Warning(
                $"FashionReportXIV returned invalid week '{state.LastOptions.Week}'.");
            return null;
        }

        FashionReportDataStorage report = new()
        {
            Week = week,
            WeeklyThemeName = state.LastOptions.ReportTitle ?? string.Empty,
            Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        foreach (FashionReportXivHint hint in state.LastOptions.Hints)
            ApplyHint(report, hint);

        if (state.DyesFresh && state.DyeData != null)
            ApplyDyes(report, state.DyeData);

        // Resolve theme/dye names to local Lumina RowIds.
        //
        // For this first migration step this also continues using the
        // existing Google Sheet for historical hint -> item mappings.
        await report.ProcessorFromString();

        CurrentState = state;

        return report;
    }

    private static void ApplyHint(
        FashionReportDataStorage report,
        FashionReportXivHint hint)
    {
        string value = hint.Hint ?? string.Empty;

        switch (hint.Slot?.ToLowerInvariant())
        {
            case "weapon":
                report.WeaponThemeName = value;
                break;

            case "head":
                report.HeadThemeName = value;
                break;

            case "body":
                report.BodyThemeName = value;
                break;

            case "hands":
            case "gloves":
                report.GlovesThemeName = value;
                break;

            case "legs":
                report.LegsThemeName = value;
                break;

            case "feet":
            case "boots":
                report.BootsThemeName = value;
                break;

            case "earrings":
            case "ears":
                report.EarringsThemeName = value;
                break;

            case "neck":
            case "necklace":
                report.NecklaceThemeName = value;
                break;

            case "wrist":
            case "bracelet":
                report.BraceletThemeName = value;
                break;

            case "rightring":
            case "right-ring":
                report.RightRingThemeName = value;
                break;

            case "leftring":
            case "left-ring":
                report.LeftRingThemeName = value;
                break;

            case "ring":
                ApplyRingHint(report, hint, value);
                break;

            default:
                LOG.Warning(
                    $"FashionReportXIV returned unknown hint slot '{hint.Slot}'.");
                break;
        }
    }

    private static void ApplyRingHint(
        FashionReportDataStorage report,
        FashionReportXivHint hint,
        string value)
    {
        switch (hint.RingNote?.ToLowerInvariant())
        {
            case "left":
                report.LeftRingThemeName = value;
                break;

            case "right":
                report.RightRingThemeName = value;
                break;

            case "both":
                report.LeftRingThemeName = value;
                report.RightRingThemeName = value;
                break;

            default:
                // We don't yet know every value FashionReportXIV may
                // use for ringNote, so don't guess.
                LOG.Warning(
                    $"FashionReportXIV ring hint '{value}' had ringNote " +
                    $"'{hint.RingNote}'.");
                break;
        }
    }

    private static void ApplyDyes(
        FashionReportDataStorage report,
        FashionReportXivDyeData dyes)
    {
        report.WeaponDyeName = dyes.Weapon?.Plus2 ?? string.Empty;
        report.HeadDyeName = dyes.Head?.Plus2 ?? string.Empty;
        report.BodyDyeName = dyes.Body?.Plus2 ?? string.Empty;
        report.GlovesDyeName = dyes.Hands?.Plus2 ?? string.Empty;
        report.LegsDyeName = dyes.Legs?.Plus2 ?? string.Empty;
        report.BootsDyeName = dyes.Feet?.Plus2 ?? string.Empty;
    }
}

internal sealed class FashionReportXivState
{
    [JsonPropertyName("lastOptions")]
    public FashionReportXivOptions? LastOptions { get; set; }

    [JsonPropertyName("dyeData")]
    public FashionReportXivDyeData? DyeData { get; set; }

    [JsonPropertyName("easy100")]
    public FashionReportXivSolution? Easy100 { get; set; }

    [JsonPropertyName("easy80")]
    public FashionReportXivSolution? Easy80 { get; set; }

    [JsonPropertyName("links")]
    public FashionReportXivLinks? Links { get; set; }

    [JsonPropertyName("dyesFresh")]
    public bool DyesFresh { get; set; }

    [JsonPropertyName("easy100Fresh")]
    public bool Easy100Fresh { get; set; }

    [JsonPropertyName("easy80Fresh")]
    public bool Easy80Fresh { get; set; }
}

internal sealed class FashionReportXivOptions
{
    [JsonPropertyName("week")]
    public string Week { get; set; } = string.Empty;

    [JsonPropertyName("reportTitle")]
    public string ReportTitle { get; set; } = string.Empty;

    [JsonPropertyName("hints")]
    public List<FashionReportXivHint> Hints { get; set; } = [];
}

internal sealed class FashionReportXivHint
{
    [JsonPropertyName("hint")]
    public string Hint { get; set; } = string.Empty;

    [JsonPropertyName("slot")]
    public string Slot { get; set; } = string.Empty;

    [JsonPropertyName("ringNote")]
    public string RingNote { get; set; } = string.Empty;
}

internal sealed class FashionReportXivDye
{
    [JsonPropertyName("plus1")]
    public string Plus1 { get; set; } = string.Empty;

    [JsonPropertyName("plus2")]
    public string Plus2 { get; set; } = string.Empty;
}

internal sealed class FashionReportXivSolution
{
    [JsonPropertyName("itemPairs")]
    public List<FashionReportXivItemPair> ItemPairs { get; set; } = [];

    [JsonPropertyName("dyes")]
    public Dictionary<string, string> Dyes { get; set; } = [];

    [JsonPropertyName("_updatedAt")]
    public long UpdatedAt { get; set; }
}

internal sealed class FashionReportXivItemPair
{
    [JsonPropertyName("slot")]
    public string Slot { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

internal sealed class FashionReportXivLinks
{
    [JsonPropertyName("theorycraft")]
    public string Theorycraft { get; set; } = string.Empty;

    [JsonPropertyName("results")]
    public string Results { get; set; } = string.Empty;
}

internal sealed class FashionReportXivDyeData
{
    [JsonPropertyName("weapon")]
    public FashionReportXivDye? Weapon { get; set; }

    [JsonPropertyName("head")]
    public FashionReportXivDye? Head { get; set; }

    [JsonPropertyName("body")]
    public FashionReportXivDye? Body { get; set; }

    [JsonPropertyName("hands")]
    public FashionReportXivDye? Hands { get; set; }

    [JsonPropertyName("legs")]
    public FashionReportXivDye? Legs { get; set; }

    [JsonPropertyName("feet")]
    public FashionReportXivDye? Feet { get; set; }

    [JsonPropertyName("_updatedAt")]
    public long UpdatedAt { get; set; }
}