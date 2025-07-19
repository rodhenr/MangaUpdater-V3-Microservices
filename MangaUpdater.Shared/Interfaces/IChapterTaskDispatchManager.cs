using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Shared.Interfaces;

public interface IChapterTaskDispatchManager
{
    TimeSpan Delay { get; set; }
    ServicesStateEnum State { get; }
    DateTime? NextExecutionUtc { get; }
    
    void Pause();
    void Resume();
    void TriggerNow();

    Task WaitForNextExecutionAsync(CancellationToken cancellationToken);
}