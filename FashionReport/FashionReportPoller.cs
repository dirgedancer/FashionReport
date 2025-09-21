using System;
using System.Timers;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace FashionReportCalculator;

internal static class FashionReportPoller
{
    private static Timer? timer;
    internal static bool IsTuesdayComplete { get; private set; }
    internal static bool IsFridayComplete { get; private set; }
    internal static event Action? OnTuesdayUpdate;
    internal static event Action? OnFridayUpdate;

    internal static void Initialize(int pollIntervalMs = 900_000)
    {
        timer = new Timer(pollIntervalMs) { AutoReset = true };
        timer.Elapsed += async (_, _) => await CheckPollersAsync();
        IsTuesdayComplete = false;
        IsFridayComplete = false;
        timer.Start();
    }

    private static async Task CheckPollersAsync()
    {
        try
        {
            uint currentWeek = CurrentWeek;
            uint currentDyeWeek = CurrentDyeWeek;

            if (currentWeek > LastCheckedWeek)
            {
                IsTuesdayComplete = false;
                IsFridayComplete = false;
                LastCheckedWeek = currentWeek;
            }

            if (!IsTuesdayComplete)
            {
                bool weekExists = await GoogleSheetData.WeekExists(currentWeek);
                if (weekExists)
                {
                    FashionReportDataStorage? temp = await GoogleSheetData.GetLatestReport();
                    if (temp == null) return;
                    IsTuesdayComplete = true;
                    if (temp!.Week > SERVICES.frdata.Week)
                        SERVICES.frdata = temp;
                    OnTuesdayUpdate?.Invoke();
                }
                return;
            }

            if (!IsFridayComplete && currentDyeWeek == currentWeek)
            {
                FashionReportDataStorage? report = await GoogleSheetData.GetLatestReport();
                if (report != null)
                {
                    int dyesFound = 0;
                    if (report.WeaponDye != null) dyesFound++;
                    if (report.HeadDye != null) dyesFound++;
                    if (report.BodyDye != null) dyesFound++;
                    if (report.GlovesDye != null) dyesFound++;
                    if (report.LegsDye != null) dyesFound++;
                    if (report.BootsDye != null) dyesFound++;
                    if (dyesFound > 0) OnFridayUpdate?.Invoke();
                    if (dyesFound >= 6) IsFridayComplete = true;
                }
            }
        }
        catch (Exception ex) { LOG.Error($"FashionReportPoller.CheckPollersAsync: {ex.Message}"); }
    }

    internal static void Dispose()
    {
        if (timer != null)
        {
            timer.Stop();
            timer.Dispose();
            timer = null;
        }
    }

    private static uint LastCheckedWeek { get; set; } = CurrentWeek;
    internal static uint CurrentWeek => (uint)((DateTime.UtcNow - new DateTime(2018, 1, 23, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7);
    internal static uint CurrentDyeWeek => (uint)((DateTime.UtcNow - new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7);
    internal static DateTime GetFridayOfDyeWeek(uint dyeWeek) => new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc).AddDays(dyeWeek * 7);
}
