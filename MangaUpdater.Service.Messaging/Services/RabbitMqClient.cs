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

    public RabbitMqClient(string hostName, string userName, string password, int port)
    {
        _factory = new ConnectionFactory
        {
            Port = port,
            HostName = hostName,
            UserName = userName,
            Password = password
        };
    }

    public async Task PublishAsync(string queueName, string message, CancellationToken ct)
    {
        await EnsureConnectionAndChannelAsync(ct);
        
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null, cancellationToken: ct);
        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body, cancellationToken: ct);
    }

    public async Task ConsumeAsync(string queueName, Func<string, Task> onMessage, CancellationToken ct)
    {
        await EnsureConnectionAndChannelAsync(ct);
        
        await _channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, cancellationToken: ct);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await onMessage(message);
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: ct);
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: ct);
        await Task.Delay(Timeout.Infinite, ct);
    }

    public async Task<bool> HasMessagesInQueueAsync(string queueName, CancellationToken ct)
    { 
        await EnsureConnectionAndChannelAsync(ct);
        
        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false, 
            cancellationToken: ct);
        
        var queueDeclare = await _channel.QueueDeclarePassiveAsync(queueName, ct);

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
    }
}