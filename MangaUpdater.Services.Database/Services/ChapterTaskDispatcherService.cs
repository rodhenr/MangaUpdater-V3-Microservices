using System.Text.Json;
using MangaUpdater.Services.Database.Feature.MangaSources;
using MangaUpdater.Services.Database.Feature.Sources;
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
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppLogger _appLogger;
    private readonly IChapterTaskDispatchManager _manager;
    private readonly Dictionary<int, SourceWorkerRegistration> _workers = new();

    public ChapterTaskDispatcherService(
        IRabbitMqClient rabbitMqClient,
        IServiceProvider serviceProvider,
        IAppLogger appLogger,
        IChapterTaskDispatchManager manager)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
        _appLogger = appLogger;
        _manager = manager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _appLogger.LogInformation("Database", "Dynamic chapter dispatcher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                var activeSources = await sender.Send(new GetActiveDispatchSourcesQuery(), stoppingToken);
                await ReconcileWorkersAsync(activeSources, stoppingToken);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Database", "Error reconciling dynamic chapter dispatch workers.", ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        await StopWorkersAsync();
        _appLogger.LogInformation("Database", "Dynamic chapter dispatcher stopping...");
    }

    private Task ReconcileWorkersAsync(IReadOnlyCollection<ActiveDispatchSourceDto> activeSources,
        CancellationToken stoppingToken)
    {
        var activeSourceIds = activeSources.Select(source => source.SourceId).ToHashSet();

        foreach (var activeSource in activeSources)
        {
            _manager.RegisterSource(activeSource.SourceId, activeSource.SourceName, activeSource.QueueName);

            if (_workers.TryGetValue(activeSource.SourceId, out var existingWorker))
            {
                if (existingWorker.SourceName == activeSource.SourceName
                    && existingWorker.QueueName == activeSource.QueueName
                    && !existingWorker.ExecutionTask.IsCompleted)
                    continue;

                existingWorker.CancellationTokenSource.Cancel();
                _workers.Remove(activeSource.SourceId);
            }

            var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            var executionTask = RunSourceLoopAsync(activeSource, linkedCancellation.Token);
            _workers[activeSource.SourceId] = new SourceWorkerRegistration(activeSource.SourceName,
                activeSource.QueueName, linkedCancellation, executionTask);
        }

        foreach (var removedSourceId in _workers.Keys.Where(sourceId => !activeSourceIds.Contains(sourceId)).ToList())
        {
            var removedWorker = _workers[removedSourceId];
            removedWorker.CancellationTokenSource.Cancel();
            _workers.Remove(removedSourceId);
            _manager.RemoveSource(removedSourceId);
        }

        return Task.CompletedTask;
    }

    private async Task RunSourceLoopAsync(ActiveDispatchSourceDto source, CancellationToken stoppingToken)
    {
        _appLogger.LogInformation("Database", "Chapters dispatcher for '{0}' ({1}) started.", source.SourceName,
            source.SourceId);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_manager.GetStateBySource(source.SourceId) == ServicesStateEnum.Paused)
            {
                await _manager.WaitForNextExecutionAsync(source.SourceId, stoppingToken);
                continue;
            }

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                var data = await sender.Send(new GetMangaSourcesToFetchQuery(source.SourceId), stoppingToken);

                _appLogger.LogInformation("Database", "[{0}] {1} mangas to fetch.", source.SourceName, data.Count);

                var tasks = data.Select(async mangaSource =>
                {
                    var hasMessages = await _rabbitMqClient.HasMessagesInQueueAsync(source.QueueName, stoppingToken);
                    if (hasMessages) return;

                    var payload = JsonSerializer.Serialize(mangaSource);
                    await _rabbitMqClient.PublishAsync(source.QueueName, payload, stoppingToken);
                    _appLogger.LogInformation("Database", "[{0}] Fetch request sent for '{1}'.", source.SourceName,
                        mangaSource.MangaName);
                });

                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Database",
                    $"[{source.SourceName}] Error during execution for source id '{source.SourceId}'.", ex);
            }

            await _manager.WaitForNextExecutionAsync(source.SourceId, stoppingToken);
        }
    }

    private async Task StopWorkersAsync()
    {
        foreach (var worker in _workers.Values)
            await worker.CancellationTokenSource.CancelAsync();

        if (_workers.Count > 0)
            await Task.WhenAll(_workers.Values.Select(worker => worker.ExecutionTask));

        _workers.Clear();
    }

    private sealed record SourceWorkerRegistration(string SourceName, string QueueName,
        CancellationTokenSource CancellationTokenSource, Task ExecutionTask);
}
