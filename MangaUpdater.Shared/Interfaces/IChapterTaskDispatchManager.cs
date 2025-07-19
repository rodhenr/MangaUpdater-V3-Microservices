using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Shared.Interfaces;

public interface IChapterTaskDispatchManager
{
    SourceDetails GetExecutionDetails(SourcesEnum source);
    ServicesStateEnum GetStateBySource(SourcesEnum source);
    void PauseBySource(SourcesEnum source);
    void ResumeBySource(SourcesEnum source);
    void TriggerNowBySource(SourcesEnum source);

    TimeSpan GetDelayBySource(SourcesEnum source);
    void SetDelayBySource(SourcesEnum source, TimeSpan delay);
    DateTime? GetNextExecutionUtcBySource(SourcesEnum source);

    Task WaitForNextExecutionAsync(SourcesEnum source, CancellationToken cancellationToken);
}