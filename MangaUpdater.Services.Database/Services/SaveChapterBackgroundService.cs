using System.Text.Json;
using MangaUpdater.Services.Database.Feature.Chapters;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Database.Services;

public class SaveChapterBackgroundService : BackgroundService
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceProvider _serviceProvider;
    public SaveChapterBackgroundService(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var saveChapters = scope.ServiceProvider.GetRequiredService<ISaveChapters>();
        
        await _rabbitMqClient.ConsumeAsync("save-chapters", async message =>
        {
            var data = JsonSerializer.Deserialize<List<FetcherChapterResultDto>>(message);
            
            await saveChapters.SaveChaptersAsync(data, stoppingToken);
        }, stoppingToken);
        
        await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
    }
}