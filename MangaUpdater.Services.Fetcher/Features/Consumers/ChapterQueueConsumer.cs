using System.Text.Json;
using MangaUpdater.Services.Fetcher.Features.Factory;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Features.Consumers;

public class ChapterQueueConsumer : BackgroundService
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IFetcher _chapterFetcher;
    private readonly FetcherFactory _factory;

    public ChapterQueueConsumer(IRabbitMqClient rabbitMqClient, IFetcher chapterFetcher, FetcherFactory factory)
    {
        _rabbitMqClient = rabbitMqClient;
        _chapterFetcher = chapterFetcher;
        _factory = factory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _rabbitMqClient.ConsumeAsync("chapters", async message =>
        {
            var mangaInfo = JsonSerializer.Deserialize<ChapterQueueMessageDto>(message);
            
            var service = _factory.GetChapterFetcher(mangaInfo.Source);
            var data = await service.GetChaptersAsync(mangaInfo, stoppingToken);
            
            await _rabbitMqClient.PublishAsync("save-chapters", JsonSerializer.Serialize(data));
        }, stoppingToken);
    }
}