using System.Text.Json;
using MangaUpdater.Services.Database.Feature.MangaSources;
using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        _appLogger.LogInformation("Database", "GetChaptersBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_manager.State == ServicesStateEnum.Paused)
            {
                _appLogger.LogInformation("Database", "Service is paused. Waiting for resume...");
                await _manager.WaitForNextExecutionAsync(stoppingToken);
                continue;
            }

            try
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

                    var payload = JsonSerializer.Serialize(mangaSource);
                    await _rabbitMqClient.PublishAsync(queueName, payload, stoppingToken);
                    _appLogger.LogInformation("Database", $"Fetch request sent for '{mangaSource.MangaName}'.");
                });

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Database", $"Error during background execution: {ex.Message}", ex);
            }

            _appLogger.LogInformation("Database", $"Waiting {_manager.Delay.TotalMinutes} minutes before next execution.");
            await _manager.WaitForNextExecutionAsync(stoppingToken);
        }

        _appLogger.LogInformation("Database", "GetChaptersBackgroundService is stopping.");
    }
}
