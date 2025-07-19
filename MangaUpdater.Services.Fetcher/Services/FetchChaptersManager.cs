using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Services;

public class FetchChaptersManager : IFetchChaptersManager
{
    public Dictionary<string, ServicesStateEnum> QueueStates { get; } = [];

    public void TryAddQueue(string queueName)
    {
        QueueStates.TryAdd(queueName, ServicesStateEnum.Running);
    }
    
    public ServicesStateEnum? GetQueueState(string queueName)
    {
        if (QueueStates.TryGetValue(queueName, out ServicesStateEnum state))
            return state;

        return null;
    }

    public void PauseQueue(string queueName)
    {
        if (!QueueStates.ContainsKey(queueName)) return;
        QueueStates[queueName] = ServicesStateEnum.Paused;
    }

    public void ResumeQueue(string queueName)
    {
        if (!QueueStates.ContainsKey(queueName)) return;
        QueueStates[queueName] = ServicesStateEnum.Running;
    }

    public void IdleQueue(string queueName)
    {
        if (!QueueStates.ContainsKey(queueName)) return;
        
        QueueStates[queueName] = ServicesStateEnum.Idle;
    }
}