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
        _appLogger.LogInformation("ChapterFetcher is starting.");
        
        await _rabbitMqClient.ConsumeAsync("get-chapters", async message =>
        {
            try
            {
                var mangaInfo = JsonSerializer.Deserialize<ChapterQueueMessageDto>(message);

                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<FetcherFactory>();

                _appLogger.LogInformation(
                    $"Fetching chapters from manga {mangaInfo.MangaId} and source {mangaInfo.Source}.");

                var fetcher = service.GetChapterFetcher(mangaInfo.Source);
                var data = await fetcher.GetChaptersAsync(mangaInfo, stoppingToken);

                _appLogger.LogInformation(
                    $"Fetched {data.Count} chapters successfully from manga {mangaInfo.MangaId} and source {mangaInfo.Source}.");

                await _rabbitMqClient.PublishAsync("save-chapters", JsonSerializer.Serialize(data), stoppingToken);
                
                _appLogger.LogInformation("Published chapters to RabbitMQ.");
            }
            catch (Exception ex)
            {
                _appLogger.LogError("An error occurred in the background service.", ex);
            }
        }, stoppingToken);
    }
}