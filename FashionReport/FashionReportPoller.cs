using System;
using System.Timers;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
#pragma warning disable IDE1006


namespace FashionReport;

internal static class FashionReportPoller
{
    private static Timer? timer;

    internal static bool IsTuesdayComplete { get; private set; }
    internal static bool IsFridayComplete { get; private set; }

    internal static event Action? OnTuesdayUpdate;
    internal static event Action? OnFridayUpdate;

    internal static void Initialize(int pollIntervalMs = 900000)
    {
        timer = new Timer(pollIntervalMs) { AutoReset = true };
        timer.Elapsed += (_, _) => CheckPollers();
        IsTuesdayComplete = false;
        IsFridayComplete = false;
    }

    internal static void Start() => timer?.Start();
    internal static void Stop() => timer?.Stop();

    private static void CheckPollers()
    {
        try
        {
            uint currentWeek = CurrentWeek;
            uint currentDyeWeek = CurrentDyeWeek;

            if (!IsTuesdayComplete && !MySql.WeekExists(currentWeek))
            {
                SERVICES.frdata = MySql.GetLatestReport() ?? new FashionReportDataStorage();
                IsTuesdayComplete = true;
                IsFridayComplete = true;
                OnTuesdayUpdate?.Invoke();
                return;
            }

            if (IsTuesdayComplete && !IsFridayComplete && currentDyeWeek == currentWeek)
            {
                int discoveredDyes = MySql.GetDiscoveredDyeCount(currentWeek);
                if (discoveredDyes > 0) OnFridayUpdate?.Invoke();
                if (discoveredDyes >= 6) IsFridayComplete = true;
            }
        }
        catch (Exception ex) { LOG.Error($"FashionReportPoller.CheckPollers: {ex.Message}"); }
    }

    internal static uint CurrentWeek => (uint)((DateTime.UtcNow - new DateTime(2018, 1, 23, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7);
    internal static uint CurrentDyeWeek => (uint)((DateTime.UtcNow - new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc)).TotalDays / 7);
    internal static DateTime GetFridayOfDyeWeek(uint dyeWeek) => new DateTime(2018, 1, 26, 8, 0, 0, DateTimeKind.Utc).AddDays(dyeWeek * 7);
}
