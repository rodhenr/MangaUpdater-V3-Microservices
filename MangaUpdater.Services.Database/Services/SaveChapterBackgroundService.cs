using System.Text.Json;
using MangaUpdater.Services.Database.Feature.Chapters;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Database.Services;

public class SaveChapterBackgroundService : BackgroundService
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppLogger _appLogger;
    
    public SaveChapterBackgroundService(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider, 
        IAppLogger appLogger)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
        _appLogger = appLogger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _appLogger.LogInformation("SaveChapters is starting.");
        
        using var scope = _serviceProvider.CreateScope();
        var saveChapters = scope.ServiceProvider.GetRequiredService<ISaveChapters>();
        
        await _rabbitMqClient.ConsumeAsync("save-chapters", async message =>
        {
            var data = JsonSerializer.Deserialize<List<FetcherChapterResultDto>>(message);
            
            _appLogger.LogInformation($"Saving {data.Count} chapters.");
            
            await saveChapters.SaveChaptersAsync(data, stoppingToken);
            
            _appLogger.LogInformation($"Saved all chapters.");
        }, stoppingToken);
    }
}