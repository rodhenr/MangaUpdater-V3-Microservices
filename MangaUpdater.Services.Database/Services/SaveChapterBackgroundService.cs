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
        using var scope = _serviceProvider.CreateScope();
        var saveChapters = scope.ServiceProvider.GetRequiredService<ISaveChapters>();
        
        await _rabbitMqClient.ConsumeAsync("save-chapters", async message =>
        {
            try
            {
                var data = JsonSerializer.Deserialize<List<FetcherChapterResultDto>>(message);

                if (data is null)
                {
                    _appLogger.LogError("Database", $"Failed to deserialize the message.");
                    return;
                }
                
                await saveChapters.SaveChaptersAsync(data, stoppingToken);
                
                _appLogger.LogInformation("Database", $"Saved {data.Count} chapters for MangaId {data.First().MangaId}.");
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Database", "Error processing message.", ex);
            }
        }, stoppingToken);
    }
}