using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Shared.Models;

public class SourceState
{
    public SourceState(SourcesEnum source)
    {
        State = ServicesStateEnum.Idle;
        Delay = TimeSpan.FromHours(3);
        PauseSemaphore = new SemaphoreSlim(0);
        Source = source;
    }
    
    public SourcesEnum Source { get; }
    public ServicesStateEnum State { get; set; }
    public TimeSpan Delay { get; set; }
    public DateTime? NextExecutionUtc { get; set; }
    public SemaphoreSlim PauseSemaphore { get; init;  }
    public TaskCompletionSource<bool>? TriggerSource  { get; set; }
    
    public SourceDetails GetDetails()
    {
        return new SourceDetails((int)Source, NextExecutionUtc, Delay, State.ToString());
    }
}