namespace MangaUpdater.Shared.Interfaces;

public interface IRabbitMqClient
{
    Task PublishAsync(string queueName, string message);
    Task ConsumeAsync(string queueName, Func<string, Task> onMessage, CancellationToken cancellationToken);
    Task<bool> HasMessagesInQueueAsync(string queueName, CancellationToken ct);
}