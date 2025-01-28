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
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var data = await sender.Send(new GetMangaSourcesToFetchQuery(), stoppingToken);

            _appLogger.LogInformation("Database", $"{data.Count} mangas to fetch.");
            
            var tasks = data.Select(async mangaSource =>
            {
                var queueName = $"get-chapters-{mangaSource.Source}";
                var hasMessages = await _rabbitMqClient.HasMessagesInQueueAsync(queueName, stoppingToken);
            
                if (hasMessages) return;

                await _rabbitMqClient.PublishAsync(queueName, JsonSerializer.Serialize(mangaSource), stoppingToken);
                _appLogger.LogInformation("Database", $"Fetch chapters request has been sent for Manga ID {mangaSource.MangaId}.");
            });

            await Task.WhenAll(tasks);

            _appLogger.LogInformation("Database", $"Delaying 3 hours before getting new chapters.");
            await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
        }
    }
}