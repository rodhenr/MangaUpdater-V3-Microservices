using System.Text.Json;
using MangaUpdater.Services.Database.Feature.MangaSources;
using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Services;

public class GetChaptersBackgroundService : BackgroundService
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceProvider _serviceProvider;
    public GetChaptersBackgroundService(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var hasMessages = await _rabbitMqClient.HasMessagesInQueueAsync("get-chapters", stoppingToken);
            
            if (hasMessages) await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
            
            using var scope = _serviceProvider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var data = await sender.Send(new GetMangaSourcesToFetchQuery(), stoppingToken);

            foreach (var mangaSource in data)
            {
                await _rabbitMqClient.PublishAsync("get-chapters", JsonSerializer.Serialize(mangaSource), stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
        }
    }
}