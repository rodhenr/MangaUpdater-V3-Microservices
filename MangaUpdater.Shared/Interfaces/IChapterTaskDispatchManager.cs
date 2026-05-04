using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Shared.Interfaces;

public interface IChapterTaskDispatchManager
{
    IReadOnlyCollection<SourceDetails> GetAllExecutionDetails();
    SourceDetails GetExecutionDetails(int sourceId);
    ServicesStateEnum GetStateBySource(int sourceId);
    void PauseBySource(int sourceId);
    void ResumeBySource(int sourceId);
    void TriggerNowBySource(int sourceId);

    TimeSpan GetDelayBySource(int sourceId);
    void SetDelayBySource(int sourceId, TimeSpan delay);
    DateTime? GetNextExecutionUtcBySource(int sourceId);
    void RegisterSource(int sourceId, string sourceName, string queueName);
    void RemoveSource(int sourceId);
    bool TryResolveSourceId(string sourceIdentifier, out int sourceId);

    Task WaitForNextExecutionAsync(int sourceId, CancellationToken cancellationToken);
}