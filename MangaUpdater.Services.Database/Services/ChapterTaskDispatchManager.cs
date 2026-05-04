using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Database.Services;

/// <summary>
/// Controls the execution flow of the chapter dispatch background services,
/// managing execution state (pause, resume, delay, trigger) separately for each manga source.
///
/// This manager is a singleton that coordinates multiple background workers,
/// each responsible for dispatching chapter fetch tasks for a specific source id.
/// </summary>
public class ChapterTaskDispatchManager : IChapterTaskDispatchManager
{
    private readonly Dictionary<int, SourceState> _states = new();
    private readonly Lock _lock = new();

    public IReadOnlyCollection<SourceDetails> GetAllExecutionDetails()
    {
        lock (_lock)
        {
            return _states.Values
                .OrderBy(state => state.SourceId)
                .Select(state => state.GetDetails())
                .ToList();
        }
    }

    public void RegisterSource(int sourceId, string sourceName, string queueName)
    {
        lock (_lock)
        {
            if (_states.TryGetValue(sourceId, out var existingState))
            {
                existingState.UpdateMetadata(sourceName, queueName);
                return;
            }

            _states[sourceId] = new SourceState(sourceId, sourceName, queueName);
        }
    }

    public void RemoveSource(int sourceId)
    {
        lock (_lock)
        {
            _states.Remove(sourceId);
        }
    }

    /// <summary>
    /// Retrieves execution information (state, delay, next run time) for the specified source.
    /// </summary>
    /// <param name="sourceId">The manga source id to inspect.</param>
    /// <returns>A <see cref="SourceDetails"/> object with execution information.</returns>
    public SourceDetails GetExecutionDetails(int sourceId) => GetState(sourceId).GetDetails();

    /// <summary>
    /// Gets the current execution state of the specified source.
    /// </summary>
    /// <param name="sourceId">The source id whose state is being queried.</param>
    /// <returns>The current <see cref="ServicesStateEnum"/> of the source.</returns>
    public ServicesStateEnum GetStateBySource(int sourceId) => GetState(sourceId).State;

    /// <summary>
    /// Pauses the background worker execution for the specified source.
    /// </summary>
    /// <param name="sourceId">The source id to pause.</param>
    public void PauseBySource(int sourceId)
    {
        GetState(sourceId).State = ServicesStateEnum.Paused;
    }

    /// <summary>
    /// Resumes the background worker execution for the specified source, if it was paused.
    /// </summary>
    /// <param name="sourceId">The source id to resume.</param>
    public void ResumeBySource(int sourceId)
    {
        var state = GetState(sourceId);
        if (state.State != ServicesStateEnum.Paused) return;

        state.State = ServicesStateEnum.Idle;
        state.PauseSemaphore.Release();
    }

    /// <summary>
    /// Triggers the background worker for the specified source to execute immediately,
    /// bypassing any configured delay.
    /// </summary>
    /// <param name="sourceId">The source id to trigger.</param>
    public void TriggerNowBySource(int sourceId)
    {
        GetState(sourceId).TriggerSource?.TrySetResult(true);
    }

    /// <summary>
    /// Gets the currently configured delay between executions for the specified source.
    /// </summary>
    /// <param name="sourceId">The source id to query.</param>
    /// <returns>The delay as a <see cref="TimeSpan"/>.</returns>
    public TimeSpan GetDelayBySource(int sourceId) => GetState(sourceId).Delay;

    /// <summary>
    /// Sets a new delay duration between executions for the specified source.
    /// </summary>
    /// <param name="sourceId">The source id to update.</param>
    /// <param name="delay">The delay duration to apply.</param>
    public void SetDelayBySource(int sourceId, TimeSpan delay) => GetState(sourceId).Delay = delay;

    /// <summary>
    /// Gets the expected UTC timestamp for the next execution cycle of the specified source.
    /// </summary>
    /// <param name="sourceId">The source id to query.</param>
    /// <returns>The next scheduled UTC execution time, or null if not scheduled.</returns>
    public DateTime? GetNextExecutionUtcBySource(int sourceId) => GetState(sourceId).NextExecutionUtc;

    public bool TryResolveSourceId(string sourceIdentifier, out int sourceId)
    {
        sourceId = 0;
        if (string.IsNullOrWhiteSpace(sourceIdentifier))
            return false;

        var candidate = sourceIdentifier.Contains('.')
            ? sourceIdentifier.Split('.').Last()
            : sourceIdentifier;

        if (int.TryParse(candidate, out sourceId))
        {
            lock (_lock)
            {
                return _states.ContainsKey(sourceId);
            }
        }

        lock (_lock)
        {
            var state = _states.Values.FirstOrDefault(current =>
                string.Equals(current.SourceName, candidate, StringComparison.OrdinalIgnoreCase)
                || string.Equals(current.QueueName, candidate, StringComparison.OrdinalIgnoreCase)
                || string.Equals($"ChapterDispatcher.{current.SourceId}", sourceIdentifier, StringComparison.OrdinalIgnoreCase)
                || string.Equals($"ChapterDispatcher.{current.SourceName}", sourceIdentifier, StringComparison.OrdinalIgnoreCase));

            if (state is null)
                return false;

            sourceId = state.SourceId;
            return true;
        }
    }


    /// <summary>
    /// Awaits the next execution cycle for the specified source, respecting pause, delay, or manual trigger.
    /// </summary>
    /// <param name="sourceId">The source id being processed.</param>
    /// <param name="cancellationToken">A token used to cancel the wait operation.</param>
    public async Task WaitForNextExecutionAsync(int sourceId, CancellationToken cancellationToken)
    {
        var state = GetState(sourceId);

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

    private SourceState GetState(int sourceId)
    {
        lock (_lock)
        {
            if (_states.TryGetValue(sourceId, out var state))
                return state;
        }

        throw new InvalidOperationException($"Source '{sourceId}' is not registered for dispatch scheduling.");
    }
}
