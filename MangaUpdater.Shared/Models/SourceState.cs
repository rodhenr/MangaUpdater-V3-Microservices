using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Shared.Models;

public class SourceState
{
    public SourceState(int sourceId, string sourceName, string queueName)
    {
        State = ServicesStateEnum.Idle;
        Delay = TimeSpan.FromHours(3);
        PauseSemaphore = new SemaphoreSlim(0);
        SourceId = sourceId;
        SourceName = sourceName;
        QueueName = queueName;
    }
    
    public int SourceId { get; }
    public string SourceName { get; private set; }
    public string QueueName { get; private set; }
    public ServicesStateEnum State { get; set; }
    public TimeSpan Delay { get; set; }
    public DateTime? NextExecutionUtc { get; set; }
    public SemaphoreSlim PauseSemaphore { get; init;  }
    public TaskCompletionSource<bool>? TriggerSource  { get; set; }

    public void UpdateMetadata(string sourceName, string queueName)
    {
        SourceName = sourceName;
        QueueName = queueName;
    }
    
    public SourceDetails GetDetails()
    {
        return new SourceDetails(SourceId, SourceName, QueueName, NextExecutionUtc, Delay, State.ToString());
    }
}