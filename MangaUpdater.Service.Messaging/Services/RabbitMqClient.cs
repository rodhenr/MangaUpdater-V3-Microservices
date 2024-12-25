using System.Text;
using MangaUpdater.Shared.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MangaUpdater.Service.Messaging.Services;

public class RabbitMqClient : IRabbitMqClient
{
    private readonly ConnectionFactory _factory;

    public RabbitMqClient(string hostName = "localhost", string userName = "guest", string password = "guest")
    {
        _factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };
    }

    public async Task PublishAsync(string queueName, string message)
    {
        await using var connection = await _factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        if (channel is null) throw new InvalidOperationException("RabbitMQ client is not connected.");

        var body = Encoding.UTF8.GetBytes(message);

        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
    }

    public async Task ConsumeAsync(string queueName, Func<string, Task> onMessage, CancellationToken ct)
    {
        await using var connection = await _factory.CreateConnectionAsync(ct);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        
        if (channel is null) throw new InvalidOperationException("RabbitMQ client is not connected.");

        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, cancellationToken: ct);
        
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await onMessage(message);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: ct);
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: ct);
        await Task.Delay(Timeout.Infinite, ct);
    }
}