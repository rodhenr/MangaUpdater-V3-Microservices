using System.Text.Json;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Services;

public class GetChaptersBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IAppLogger _appLogger;

    public GetChaptersBackgroundService(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider, 
        IAppLogger appLogger)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
        _appLogger = appLogger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _rabbitMqClient.ConsumeAsync("get-chapters", async message =>
        {
            try
            {
                var mangaInfo = JsonSerializer.Deserialize<ChapterQueueMessageDto>(message);

                if (mangaInfo is null)
                {
                    _appLogger.LogError("Fetcher", "Failed to deserialize the message.");
                    return;
                }
                
                _appLogger.LogInformation("Fetcher", $"Fetching chapters: Manga ID {mangaInfo.MangaId} from '{mangaInfo.Source}'.");

                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<FetcherFactory>();
                var fetcher = service.GetChapterFetcher(mangaInfo.Source);
                
                var data = await fetcher.GetChaptersAsync(mangaInfo, stoppingToken);
                
                if (data.Count == 0)
                {
                    _appLogger.LogInformation("Fetcher", $"No chapters to save.");
                    return;
                }

                await _rabbitMqClient.PublishAsync("save-chapters", JsonSerializer.Serialize(data), 
                    stoppingToken);
                
                _appLogger.LogInformation("Fetcher", $"Queued for processing: {data.Count} chapters for Manga ID = {mangaInfo.MangaId} from '{mangaInfo.Source}'.");
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Fetcher", "Error processing message.", ex);
            }
        }, stoppingToken);
    }
}