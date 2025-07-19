using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Database.Services;

/// <summary>
/// Controls the execution flow of the chapter dispatch background services,
/// managing execution state (pause, resume, delay, trigger) separately for each manga source.
///
/// This manager is a singleton that coordinates multiple background workers,
/// each responsible for dispatching chapter fetch tasks for a specific <see cref="SourcesEnum"/>.
/// </summary>
public class ChapterTaskDispatchManager : IChapterTaskDispatchManager
{
    private readonly Dictionary<SourcesEnum, SourceState> _states = new();

    public ChapterTaskDispatchManager()
    {
        foreach (var source in Enum.GetValues<SourcesEnum>())
        {
            _states[source] = new SourceState(source);
        }
    }

    /// <summary>
    /// Retrieves execution information (state, delay, next run time) for the specified source.
    /// </summary>
    /// <param name="source">The manga source to inspect.</param>
    /// <returns>A <see cref="SourceDetails"/> object with execution information.</returns>
    public SourceDetails GetExecutionDetails(SourcesEnum source) => _states[source].GetDetails();

    /// <summary>
    /// Gets the current execution state of the specified source.
    /// </summary>
    /// <param name="source">The source whose state is being queried.</param>
    /// <returns>The current <see cref="ServicesStateEnum"/> of the source.</returns>
    public ServicesStateEnum GetStateBySource(SourcesEnum source) => _states[source].State;

    /// <summary>
    /// Pauses the background worker execution for the specified source.
    /// </summary>
    /// <param name="source">The source to pause.</param>
    public void PauseBySource(SourcesEnum source)
    {
        _states[source].State = ServicesStateEnum.Paused;
    }

    /// <summary>
    /// Resumes the background worker execution for the specified source, if it was paused.
    /// </summary>
    /// <param name="source">The source to resume.</param>
    public void ResumeBySource(SourcesEnum source)
    {
        var state = _states[source];
        if (state.State != ServicesStateEnum.Paused) return;

        state.State = ServicesStateEnum.Idle;
        state.PauseSemaphore.Release();
    }

    /// <summary>
    /// Triggers the background worker for the specified source to execute immediately,
    /// bypassing any configured delay.
    /// </summary>
    /// <param name="source">The source to trigger.</param>
    public void TriggerNowBySource(SourcesEnum source)
    {
        _states[source].TriggerSource?.TrySetResult(true);
    }

    /// <summary>
    /// Gets the currently configured delay between executions for the specified source.
    /// </summary>
    /// <param name="source">The source to query.</param>
    /// <returns>The delay as a <see cref="TimeSpan"/>.</returns>
    public TimeSpan GetDelayBySource(SourcesEnum source) => _states[source].Delay;

    /// <summary>
    /// Sets a new delay duration between executions for the specified source.
    /// </summary>
    /// <param name="source">The source to update.</param>
    /// <param name="delay">The delay duration to apply.</param>
    public void SetDelayBySource(SourcesEnum source, TimeSpan delay) => _states[source].Delay = delay;

    /// <summary>
    /// Gets the expected UTC timestamp for the next execution cycle of the specified source.
    /// </summary>
    /// <param name="source">The source to query.</param>
    /// <returns>The next scheduled UTC execution time, or null if not scheduled.</returns>
    public DateTime? GetNextExecutionUtcBySource(SourcesEnum source) => _states[source].NextExecutionUtc;


    /// <summary>
    /// Awaits the next execution cycle for the specified source, respecting pause, delay, or manual trigger.
    /// </summary>
    /// <param name="source">The source being processed.</param>
    /// <param name="cancellationToken">A token used to cancel the wait operation.</param>
    public async Task WaitForNextExecutionAsync(SourcesEnum source, CancellationToken cancellationToken)
    {
        var state = _states[source];

        if (state.State == ServicesStateEnum.Paused)
        {
            await state.PauseSemaphore.WaitAsync(cancellationToken);
        }

        state.State = ServicesStateEnum.Idle;
        state.TriggerSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var delayTask = Task.Delay(state.Delay, cancellationToken);
        state.NextExecutionUtc = DateTime.UtcNow + state.Delay;

        await Task.WhenAny(delayTask, state.TriggerSource.Task);

        state.TriggerSource = null;

        cancellationToken.ThrowIfCancellationRequested();
    }
}
