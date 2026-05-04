using System.Text.Json;
using MangaUpdater.Services.Database.Database;
using Microsoft.EntityFrameworkCore;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Services;

public class FetchChaptersService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IAppLogger _appLogger;
    private readonly IFetchChaptersManager _manager;
    private readonly Dictionary<int, QueueConsumerRegistration> _consumers = new();

    public FetchChaptersService(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider, 
        IAppLogger appLogger, IFetchChaptersManager manager)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
        _appLogger = appLogger;
        _manager = manager;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ReconcileConsumersAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _appLogger.LogError("Fetcher", "Error setting up consumers for chapter queues.", ex);
        }
        finally
        {
            await StopConsumersAsync();
        }
    }

    private async Task ReconcileConsumersAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var activeSources = await dbContext.Sources
            .AsNoTracking()
            .Where(source => source.IsEnabled)
            .OrderBy(source => source.Id)
            .Select(source => new
            {
                source.Id,
                QueueName = string.IsNullOrWhiteSpace(source.QueueName)
                    ? $"get-chapters-{source.Slug ?? source.Id.ToString()}"
                    : source.QueueName
            })
            .ToListAsync(stoppingToken);

        var activeSourceIds = activeSources.Select(source => source.Id).ToHashSet();

        foreach (var activeSource in activeSources)
        {
            if (_consumers.TryGetValue(activeSource.Id, out var existingConsumer))
            {
                if (existingConsumer.QueueName == activeSource.QueueName && !existingConsumer.ConsumerTask.IsCompleted)
                    continue;

                await existingConsumer.CancellationTokenSource.CancelAsync();
                _consumers.Remove(activeSource.Id);
            }

            _manager.TryAddQueue(activeSource.QueueName);
            
            var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            var consumerTask = _rabbitMqClient.ConsumeAsync(activeSource.QueueName,
                message => HandleMessageAsync(message, activeSource.QueueName, linkedCancellation.Token), linkedCancellation.Token);
            
            _consumers[activeSource.Id] = new QueueConsumerRegistration(activeSource.QueueName, linkedCancellation, consumerTask);
        }

        foreach (var removedSourceId in _consumers.Keys.Where(sourceId => !activeSourceIds.Contains(sourceId)).ToList())
        {
            await _consumers[removedSourceId].CancellationTokenSource.CancelAsync();
            _consumers.Remove(removedSourceId);
        }
    }

    private async Task<bool> HandleMessageAsync(string message, string queueName, CancellationToken stoppingToken)
    {
        try
        {
            if (_manager.GetQueueState(queueName) == ServicesStateEnum.Paused) return false;
            
            _manager.ResumeQueue(queueName);

            var mangaInfo = DeserializeMessage(message);
            if (mangaInfo is null) return true;

            var chapters = await FetchChaptersAsync(mangaInfo, stoppingToken);
            if (chapters.Count == 0) return true;

            await PublishChaptersAsync(chapters, stoppingToken);
            await UpdateManagerStateAsync(queueName, stoppingToken);

            return true;
        }
        catch (Exception ex)
        {
            _appLogger.LogError("Fetcher", "Error processing message.", ex);
            return true;
        }
    }

    private ChapterQueueMessageDto? DeserializeMessage(string message)
    {
        var dto = JsonSerializer.Deserialize<ChapterQueueMessageDto>(message);
        if (dto is null)
            _appLogger.LogError("Fetcher", "Failed to deserialize the message.");
        else
            _appLogger.LogInformation("Fetcher", $"Fetching chapters: Manga '{dto.MangaName}' from '{dto.SourceSlug ?? dto.SourceId.ToString()}'.");
        
        return dto;
    }

    private async Task<List<ChapterResult>> FetchChaptersAsync(ChapterQueueMessageDto dto, CancellationToken token)
    {
        using var scope = _serviceProvider.CreateScope();
        var orchestrator = scope.ServiceProvider.GetRequiredService<ScraperOrchestrator>();

        return await orchestrator.FetchAsync(dto, token);
    }

    private async Task PublishChaptersAsync(List<ChapterResult> chapters, CancellationToken token)
    {
        await _rabbitMqClient.PublishAsync("save-chapters", JsonSerializer.Serialize(chapters), token);
        _appLogger.LogInformation("Fetcher", $"Queued for processing: {chapters.Count} chapters.");
    }

    private async Task UpdateManagerStateAsync(string queueName, CancellationToken token)
    {
        var hasMessages = await _rabbitMqClient.HasMessagesInQueueAsync(queueName, token);
        if (!hasMessages) _manager.IdleQueue(queueName);
    }

    private async Task StopConsumersAsync()
    {
        foreach (var consumer in _consumers.Values)
            await consumer.CancellationTokenSource.CancelAsync();

        if (_consumers.Count > 0)
            await Task.WhenAll(_consumers.Values.Select(consumer => consumer.ConsumerTask));

        _consumers.Clear();
    }

    private sealed record QueueConsumerRegistration(string QueueName, CancellationTokenSource CancellationTokenSource, 
        Task ConsumerTask);
}