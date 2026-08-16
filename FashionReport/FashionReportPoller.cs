using System;
using System.Timers;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace FashionReportCalculator;

internal static class FashionReportPoller
{
    private static Timer? timer;
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromMinutes(15);

    internal static void Initialize(int pollIntervalMs = 1_000)
    {
        timer = new Timer(pollIntervalMs) { AutoReset = false };
        timer.Elapsed += OnTimerElapsed;
        timer.Start();
    }

    private static async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (timer == null) return;
        TimeSpan nextInterval = DefaultInterval;
        try { nextInterval = await CheckPollers(); }
        catch (Exception ex) { LOG.Error($"FashionReportPoller.CheckPollers: {ex.Message}"); }
        finally
        {
            double ms = Math.Min(Math.Max(nextInterval.TotalMilliseconds, 1000), int.MaxValue);
            timer.Interval = ms;
            timer.Start();
        }
    }

    private static async Task<TimeSpan> CheckPollers()
    {
        try
        {
            FashionReportDataStorage? report =
                await FashionReportXivProvider.GetCurrentReportAsync();
            if (report == null) return DefaultInterval;
            if (report.Week > SERVICES.frdata.Week)
            {
                SERVICES.frdata = report;
                FashionReport.DisplayWindow.RefreshDisplayData();
                LOG.Info(
                    $"FashionReportXIV updated to week {report.Week}: " +
                    $"{report.WeeklyThemeName}");

                if (GetFridayOfDyeWeek(CurrentWeek) > DateTime.UtcNow)
                    return WaitForFriday();
            }
            else if (CurrentWeek == SERVICES.frdata.Week && GetFridayOfDyeWeek(CurrentWeek) > DateTime.UtcNow)
            {
                LOG.Info("Tuesday Server and Client updated, waiting until Friday");
                return WaitForFriday();
            }
            int dyesFound = 0;
            if (report.WeaponDye != null) dyesFound++;
            if (report.HeadDye != null) dyesFound++;
            if (report.BodyDye != null) dyesFound++;
            if (report.GlovesDye != null) dyesFound++;
            if (report.LegsDye != null) dyesFound++;
            if (report.BootsDye != null) dyesFound++;
            if (report != SERVICES.frdata)
            {
                SERVICES.frdata = report;
                FashionReport.DisplayWindow.RefreshDisplayData();
                LOG.Info($"Dyes known: {dyesFound}");
            }
            if (dyesFound == 6)
            {
                LOG.Info($"All 6 Dyes found this week, delaying for {WaitForNewWeek()}");
                return WaitForNewWeek();
            }
            return DefaultInterval;
        }
        catch (Exception ex)
        {
            LOG.Error($"FashionReportPoller.CheckPollers: {ex.Message}");
            return DefaultInterval;
        }
    }

    internal static void Dispose()
    {
        if (timer != null)
        {
            timer.Elapsed -= OnTimerElapsed;
            timer.Stop();
            timer.Dispose();
            timer = null;
        }
    }

    private static TimeSpan WaitForFriday() => GetFridayOfDyeWeek(CurrentWeek) > DateTime.UtcNow ? GetFridayOfDyeWeek(CurrentWeek) - DateTime.UtcNow : DefaultInterval;
    private static TimeSpan WaitForNewWeek() => GetTuesdayNextWeek() > DateTime.UtcNow ? GetTuesdayNextWeek() - DateTime.UtcNow : DefaultInterval;
    internal static uint CurrentWeek => (uint)((DateTime.UtcNow - new DateTime(2018, 1, 23, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7);
    internal static DateTime GetFridayOfDyeWeek(uint dyeWeek) => new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc).AddDays(dyeWeek * 7);
    internal static DateTime GetTuesdayNextWeek() => new DateTime(2018, 1, 23, 8, 0, 0, DateTimeKind.Utc).AddDays((CurrentWeek + 1) * 7);
}
