using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Shared.Interfaces;

public interface IFetchChaptersManager
{
    Dictionary<string, ServicesStateEnum> QueueStates { get; }

    void TryAddQueue(string queueName);
    ServicesStateEnum? GetQueueState(string queueName);
    void PauseQueue(string queueName);
    void ResumeQueue(string queueName);
    void IdleQueue(string queueName);
}