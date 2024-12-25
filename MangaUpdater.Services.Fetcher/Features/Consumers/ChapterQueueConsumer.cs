using System.Text.Json;
using MangaUpdater.Services.Fetcher.Features.Factory;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Features.Consumers;

public class ChapterQueueConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqClient _rabbitMqClient;

    public ChapterQueueConsumer(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _rabbitMqClient.ConsumeAsync("get-chapters", async message =>
        {
            var mangaInfo = JsonSerializer.Deserialize<ChapterQueueMessageDto>(message);
            
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<FetcherFactory>();
            
            var fetcher = service.GetChapterFetcher(mangaInfo.Source);
            var data = await fetcher.GetChaptersAsync(mangaInfo, stoppingToken);
            
            await _rabbitMqClient.PublishAsync("save-chapters", JsonSerializer.Serialize(data));
        }, stoppingToken);
    }
}