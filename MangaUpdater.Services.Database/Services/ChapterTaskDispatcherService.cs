using System.Text.Json;
using MangaUpdater.Services.Database.Feature.MangaSources;
using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Services;

/// <summary>
/// Background service that prepares and dispatches chapter fetch tasks
/// to RabbitMQ queues. It retrieves manga-source associations that need
/// updates and publishes them as tasks for further processing.
/// Controlled by an execution manager that allows pausing, resuming,
/// triggering, and setting delays.
/// </summary>
public class ChapterTaskDispatcherService : BackgroundService
{
    private readonly SourcesEnum _source;
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppLogger _appLogger;
    private readonly IChapterTaskDispatchManager _manager;
    private readonly string _queueName;

    public ChapterTaskDispatcherService(
        SourcesEnum source,
        IRabbitMqClient rabbitMqClient,
        IServiceProvider serviceProvider,
        IAppLogger appLogger,
        IChapterTaskDispatchManager manager)
    {
        _source = source;
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
        _appLogger = appLogger;
        _manager = manager;
        _queueName = $"get-chapters-{_source}";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _appLogger.LogInformation("Database", $"Chapters dispatcher for '{_source}' started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_manager.GetStateBySource(_source) == ServicesStateEnum.Paused)
            {
                await _manager.WaitForNextExecutionAsync(_source, stoppingToken);
                continue;
            }

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                
                var data = await sender.Send(new GetMangaSourcesToFetchQuery(_source), stoppingToken);

                _appLogger.LogInformation("Database", $"[{_source}] {data.Count} mangas to fetch.");

                var tasks = data.Select(async mangaSource =>
                {
                    var hasMessages = await _rabbitMqClient.HasMessagesInQueueAsync(_queueName, stoppingToken);
                    if (hasMessages) return;

                    var payload = JsonSerializer.Serialize(mangaSource);
                    await _rabbitMqClient.PublishAsync(_queueName, payload, stoppingToken);
                    _appLogger.LogInformation("Database", $"[{_source}] Fetch request sent for '{mangaSource.MangaName}'.");
                });

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Database", $"[{_source}] Error during execution: {ex.Message}", ex);
            }

            await _manager.WaitForNextExecutionAsync(_source, stoppingToken);
        }

        _appLogger.LogInformation("Database", $"Chapters dispatcher for {_source} stopping...");
    }
}
