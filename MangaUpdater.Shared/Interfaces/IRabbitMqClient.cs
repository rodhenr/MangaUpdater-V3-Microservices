namespace MangaUpdater.Shared.Interfaces;

public interface IRabbitMqClient
{
    Task PublishAsync(string queueName, string message, CancellationToken ct);
    Task ConsumeAsync(string queueName, Func<string, Task<bool>> onMessage, CancellationToken ct);
    Task<bool> HasMessagesInQueueAsync(string queueName, CancellationToken ct);
}