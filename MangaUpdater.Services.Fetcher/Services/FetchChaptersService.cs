using System.Text.Json;
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
            var tasks = Enum.GetValues<SourcesEnum>().Select(source =>
            {
                var queueName = $"get-chapters-{source}";
                _manager.TryAddQueue(queueName);
                
                return _rabbitMqClient.ConsumeAsync(queueName, message =>
                    HandleMessageAsync(message, queueName, stoppingToken), stoppingToken);
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _appLogger.LogError("Fetcher", "Error setting up consumers for chapter queues.", ex);
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
            _appLogger.LogInformation("Fetcher", $"Fetching chapters: Manga '{dto.MangaName}' from '{dto.Source}'.");
        
        return dto;
    }

    private async Task<List<ChapterResult>> FetchChaptersAsync(ChapterQueueMessageDto dto, CancellationToken token)
    {
        using var scope = _serviceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<FetcherFactory>();
        var fetcher = factory.GetChapterFetcher(dto.Source);
        
        return await fetcher.GetChaptersAsync(dto, token);
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

}