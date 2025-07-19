namespace MangaUpdater.Shared.Enums;

/// <summary>
/// Possible execution states of the GetChaptersBackgroundService.
/// </summary>
public enum ServicesStateEnum
{
    /// <summary>Service is currently processing the work.</summary>
    Running,

    /// <summary>Service is paused and waiting indefinitely.</summary>
    Paused,

    /// <summary>Service is waiting for the next scheduled execution (delay or manual trigger).</summary>
    Idle
}