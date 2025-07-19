using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Database.Services;

/// <summary>
/// Controls the execution flow of the GetChaptersBackgroundService, 
/// allowing it to be paused, resumed, delayed, or triggered manually.
/// </summary>
public class ChapterTaskDispatchManager : IChapterTaskDispatchManager
{
    private readonly SemaphoreSlim _pauseSemaphore = new(0);
    private TaskCompletionSource<bool>? _triggerSource;

    /// <summary>
    /// Represents the current execution state of the GetChaptersBackgroundService.
    /// </summary>
    public ServicesStateEnum State { get; private set; } = ServicesStateEnum.Idle;

    /// <summary>
    /// Gets or sets the delay between background service executions.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.FromHours(3);

    /// <summary>
    /// Gets the expected UTC datetime for the next execution, if not paused or manually triggered.
    /// </summary>
    public DateTime? NextExecutionUtc { get; private set; }

    /// <summary>
    /// Pauses the background service execution.
    /// </summary>
    public void Pause()
    {
        State = ServicesStateEnum.Paused;
    }

    /// <summary>
    /// Resumes the background service execution if it was paused.
    /// </summary>
    public void Resume()
    {
        if (State != ServicesStateEnum.Paused) return;
        
        State = ServicesStateEnum.Idle;
        _pauseSemaphore.Release();
    }

    /// <summary>
    /// Triggers the background service to execute immediately,
    /// interrupting any current delay.
    /// </summary>
    public void TriggerNow()
    {
        _triggerSource?.TrySetResult(true);
    }

    /// <summary>
    /// Waits for the next execution cycle, considering pause state, delay, or manual trigger.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the wait.</param>
    public async Task WaitForNextExecutionAsync(CancellationToken cancellationToken)
    {
        if (State == ServicesStateEnum.Paused)
        {
            await _pauseSemaphore.WaitAsync(cancellationToken);
        }

        State = ServicesStateEnum.Idle;
        _triggerSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var delayTask = Task.Delay(Delay, cancellationToken);
        NextExecutionUtc = DateTime.UtcNow + Delay;

        var completedTask = await Task.WhenAny(delayTask, _triggerSource.Task);

        _triggerSource = null;

        cancellationToken.ThrowIfCancellationRequested();
    }
}