using System.Text;
using MangaUpdater.Shared.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MangaUpdater.Service.Messaging.Services;

public class RabbitMqClient : IRabbitMqClient, IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqClient(string hostname, string username, string password, int port)
    {
        _factory = new ConnectionFactory
        {
            Port = port,
            HostName = hostname,
            UserName = username,
            Password = password
        };
    }

    public async Task PublishAsync(string queueName, string message, CancellationToken ct)
    {
        await EnsureConnectionAndChannelAsync(ct);
        await EnsureQueueDeclaredAsync(queueName, ct);
        
        var body = Encoding.UTF8.GetBytes(message);
        await _channel!.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body, cancellationToken: ct);
    }

    public async Task ConsumeAsync(string queueName, Func<string, Task<bool>> onMessage, CancellationToken ct)
    {
        await EnsureConnectionAndChannelAsync(ct);
        await EnsureQueueDeclaredAsync(queueName, ct);
        
        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                Console.WriteLine($"[INFO] Received message: {message}");
                var result = await onMessage(message);
                
                if (result) 
                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: ct);
                else 
                    await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, true, cancellationToken: ct);
                
                Console.WriteLine("[INFO] Message acknowledged.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to process message: {ex.Message}");
                await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true, cancellationToken: ct);
            }
        };

        await _channel!.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: ct);
        await Task.Delay(Timeout.Infinite, ct);
    }

    public async Task<bool> HasMessagesInQueueAsync(string queueName, CancellationToken ct)
    { 
        await EnsureConnectionAndChannelAsync(ct);
        await EnsureQueueDeclaredAsync(queueName, ct);
        
        var queueDeclare = await _channel!.QueueDeclarePassiveAsync(queueName, ct);
        return queueDeclare.MessageCount > 0;
    }

    private async Task EnsureConnectionAndChannelAsync(CancellationToken ct)
    {
        if (_connection is null || !_connection.IsOpen)
        {
            _connection = await _factory.CreateConnectionAsync(ct);
        }

        if (_channel is null || !_channel.IsOpen)
        {
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        
            if (_channel is null) throw new InvalidOperationException("RabbitMQ client is not connected.");
        }
    }

    private async Task EnsureQueueDeclaredAsync(string queueName, CancellationToken ct)
    {
        await _channel!.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, cancellationToken: ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        
        GC.SuppressFinalize(this);
    }
}