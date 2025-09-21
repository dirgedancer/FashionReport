using System;
using System.Timers;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace FashionReportCalculator;

internal static class FashionReportPoller
{
    private static Timer? timer;
    //internal static event Action? OnTuesdayUpdate;
    //internal static event Action? OnFridayUpdate;

    internal static void Initialize(int pollIntervalMs = 900_000)
    {
        timer = new Timer(pollIntervalMs) { AutoReset = true };
        timer.Elapsed += async (_, _) => await CheckPollers();
        timer.Start();
    }

    private static async Task CheckPollers()
    {
        try
        {
            if (CurrentWeek > SERVICES.frdata.Week)
            {
                bool Updated = await GoogleSheetData.WeekExists(CurrentWeek);
                if (Updated)
                {
                    FashionReportDataStorage? report = await GoogleSheetData.GetLatestReport();
                    if (report == null) return;
                    if (report.Week > SERVICES.frdata.Week)
                    {
                        SERVICES.frdata = report;
                        LOG.Info("Tuesday Server and Client updated, waiting until Friday");
                        await Task.Delay(WaitForFriday());
                    }
                }
            }
            else if ((CurrentWeek == SERVICES.frdata.Week) && (GetFridayOfDyeWeek(CurrentWeek) > DateTime.Now))
            {
                LOG.Info("Tuesday Server and Client updated, waiting until Friday");
                await Task.Delay(WaitForFriday());
            }
            else
            {
                FashionReportDataStorage? report = await GoogleSheetData.GetLatestReport();
                if (report == null) return;
                int dyesFound = 0;
                if (report.WeaponDye != null) dyesFound++;
                if (report.HeadDye != null) dyesFound++;
                if (report.BodyDye != null) dyesFound++;
                if (report.GlovesDye != null) dyesFound++;
                if (report.LegsDye != null) dyesFound++;
                if (report.BootsDye != null) dyesFound++;
                await report.ProcessFromId();
                if (report != SERVICES.frdata)
                {
                    SERVICES.frdata = report;
                    LOG.Info("Updated Dye(s)");
                }
                //if (dyesFound > 0) OnFridayUpdate?.Invoke();
                if (dyesFound >= 6)
                    await Task.Delay(WaitForNewWeek());
            }
        }
        catch (Exception ex) { LOG.Error($"FashionReportPoller.CheckPollers: {ex.Message}"); }
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

    private static TimeSpan WaitForFriday() => GetFridayOfDyeWeek(CurrentWeek) > DateTime.Now ? GetFridayOfDyeWeek(CurrentWeek) - DateTime.Now : TimeSpan.Zero;
    private static TimeSpan WaitForNewWeek() => GetTuesdayNextWeek() > DateTime.Now ? GetTuesdayNextWeek() - DateTime.Now : TimeSpan.Zero;
    internal static uint CurrentWeek => (uint)((DateTime.UtcNow - new DateTime(2018, 1, 23, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7);
    internal static uint CurrentDyeWeek => (uint)((DateTime.UtcNow - new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7);
    internal static DateTime GetFridayOfDyeWeek(uint dyeWeek) => new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc).AddDays(dyeWeek * 7);
    internal static DateTime GetTuesdayNextWeek() => new DateTime(2018, 1, 23, 8, 0, 0, DateTimeKind.Utc).AddDays((CurrentWeek + 1) * 7);
}
