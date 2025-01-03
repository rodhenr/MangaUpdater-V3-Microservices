using System.Text.Json;
using MangaUpdater.Services.Database.Feature.MangaSources;
using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Services;

public class GetChaptersBackgroundService : BackgroundService
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceProvider _serviceProvider;
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
        _appLogger.LogInformation("GetChapters is starting.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var hasMessages = await _rabbitMqClient.HasMessagesInQueueAsync("get-chapters", stoppingToken);
            
            if (hasMessages) await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
            
            using var scope = _serviceProvider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var data = await sender.Send(new GetMangaSourcesToFetchQuery(), stoppingToken);

            _appLogger.LogInformation($"{data.Count} mangas to fetch.");
            
            foreach (var mangaSource in data)
            {
                await _rabbitMqClient.PublishAsync("get-chapters", JsonSerializer.Serialize(mangaSource), stoppingToken);
                _appLogger.LogInformation("Published chapters to fetch in RabbitMQ.");
            }

            _appLogger.LogInformation("Delaying 3 hours before getting new chapters.");
            await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
        }
    }
}