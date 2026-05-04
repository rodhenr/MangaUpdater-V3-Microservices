using System.Collections.Concurrent;
using System.Net;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Services;

public class SourceRequestExecutor : ISourceRequestExecutor
{
    private const int MaxAttempts = 3;
    private const int CooldownFailureThreshold = 3;
    private static readonly TimeSpan BaseRetryDelay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan DefaultCooldown = TimeSpan.FromSeconds(30);

    private readonly HttpClient _httpClient;
    private readonly IAppLogger _appLogger;
    private readonly ConcurrentDictionary<int, SourceRequestState> _states = new();

    public SourceRequestExecutor(IHttpClientFactory httpClientFactory, IAppLogger appLogger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _appLogger = appLogger;
    }

    public async Task<string> GetStringAsync(SourceRuntimeDefinition sourceRuntimeDefinition, string requestUrl,
        CancellationToken cancellationToken = default)
    {
        var state = _states.GetOrAdd(sourceRuntimeDefinition.SourceId, _ => new SourceRequestState());
        await state.Gate.WaitAsync(cancellationToken);

        try
        {
            EnsureNotCoolingDown(sourceRuntimeDefinition, state);
            await RespectRateLimitAsync(sourceRuntimeDefinition, state, cancellationToken);

            Exception? lastException = null;

            for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                using var request = BuildRequest(sourceRuntimeDefinition, requestUrl);
                using var timeoutCts = BuildTimeoutToken(sourceRuntimeDefinition, cancellationToken);

                try
                {
                    using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                        timeoutCts?.Token ?? cancellationToken);

                    state.LastRequestUtc = DateTime.UtcNow;

                    if (IsTransientStatusCode(response.StatusCode) && attempt < MaxAttempts)
                    {
                        await DelayRetryAsync(sourceRuntimeDefinition, attempt, requestUrl, response.StatusCode.ToString(),
                            cancellationToken);
                        continue;
                    }

                    if (IsTransientStatusCode(response.StatusCode))
                    {
                        var statusException = new HttpRequestException(
                            $"Remote source responded with transient status code '{response.StatusCode}'.");
                        RegisterFailure(sourceRuntimeDefinition, state, statusException.Message);
                        throw statusException;
                    }

                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }
                catch (Exception ex) when (IsTransientException(ex) && attempt < MaxAttempts)
                {
                    state.LastRequestUtc = DateTime.UtcNow;
                    lastException = ex;
                    await DelayRetryAsync(sourceRuntimeDefinition, attempt, requestUrl, ex.Message, cancellationToken);
                }
                catch (Exception ex) when (IsTransientException(ex))
                {
                    state.LastRequestUtc = DateTime.UtcNow;
                    RegisterFailure(sourceRuntimeDefinition, state, ex.Message);
                    throw;
                }
            }

            RegisterFailure(sourceRuntimeDefinition, state, lastException?.Message ?? "Unknown HTTP failure.");
            throw lastException ?? new InvalidOperationException("Unknown HTTP failure.");
        }
        finally
        {
            state.Gate.Release();
        }
    }

    public void RegisterSuccess(SourceRuntimeDefinition sourceRuntimeDefinition)
    {
        var state = _states.GetOrAdd(sourceRuntimeDefinition.SourceId, _ => new SourceRequestState());
        state.ConsecutiveFailures = 0;
        state.CooldownUntilUtc = null;
    }

    public void RegisterConfigurationFailure(SourceRuntimeDefinition sourceRuntimeDefinition, string reason)
    {
        var state = _states.GetOrAdd(sourceRuntimeDefinition.SourceId, _ => new SourceRequestState());
        RegisterFailure(sourceRuntimeDefinition, state, reason, "configuration");
    }

    private async Task DelayRetryAsync(SourceRuntimeDefinition sourceRuntimeDefinition, int attempt, string requestUrl,
        string reason, CancellationToken cancellationToken)
    {
        var retryDelay = TimeSpan.FromMilliseconds(BaseRetryDelay.TotalMilliseconds * attempt);
        _appLogger.LogWarning("Fetch",
            "Transient HTTP failure for source '{0}' ({1}) engine '{2}' profile '{3}' on attempt {4}/{5}. Retrying in {6} ms. Url: {7}. Reason: {8}",
            sourceRuntimeDefinition.SourceName,
            sourceRuntimeDefinition.SourceId,
            sourceRuntimeDefinition.EngineType,
            ResolveProfileVersion(sourceRuntimeDefinition),
            attempt,
            MaxAttempts,
            (int)retryDelay.TotalMilliseconds,
            requestUrl,
            reason);
        await Task.Delay(retryDelay, cancellationToken);
    }

    private static CancellationTokenSource? BuildTimeoutToken(SourceRuntimeDefinition sourceRuntimeDefinition,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(sourceRuntimeDefinition.RequestProfile.GetValueOrDefault("TimeoutSeconds"), out var timeoutSeconds) ||
            timeoutSeconds <= 0)
            return null;

        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return timeoutCts;
    }

    private static HttpRequestMessage BuildRequest(SourceRuntimeDefinition sourceRuntimeDefinition, string requestUrl)
    {
        var method = sourceRuntimeDefinition.RequestProfile.GetValueOrDefault("Method") ?? HttpMethod.Get.Method;
        var request = new HttpRequestMessage(new HttpMethod(method), requestUrl);

        foreach (var header in sourceRuntimeDefinition.RequestProfile
                     .Where(entry => entry.Key.StartsWith("Header:", StringComparison.OrdinalIgnoreCase) &&
                                     !string.IsNullOrWhiteSpace(entry.Value)))
        {
            request.Headers.TryAddWithoutValidation(header.Key[7..], header.Value);
        }

        if (!string.IsNullOrWhiteSpace(sourceRuntimeDefinition.DefaultUserAgent) &&
            !request.Headers.Contains("User-Agent"))
            request.Headers.TryAddWithoutValidation("User-Agent", sourceRuntimeDefinition.DefaultUserAgent);

        var acceptLanguage = sourceRuntimeDefinition.RequestProfile.GetValueOrDefault("AcceptLanguage");
        if (!string.IsNullOrWhiteSpace(acceptLanguage) && !request.Headers.Contains("Accept-Language"))
            request.Headers.TryAddWithoutValidation("Accept-Language", acceptLanguage);

        var referrer = sourceRuntimeDefinition.RequestProfile.GetValueOrDefault("Referrer");
        if (!string.IsNullOrWhiteSpace(referrer) && Uri.TryCreate(referrer, UriKind.Absolute, out var referrerUri))
            request.Headers.Referrer = referrerUri;

        return request;
    }

    private async Task RespectRateLimitAsync(SourceRuntimeDefinition sourceRuntimeDefinition, SourceRequestState state,
        CancellationToken cancellationToken)
    {
        if (sourceRuntimeDefinition.RateLimitMilliseconds is not > 0)
            return;

        if (!state.LastRequestUtc.HasValue)
            return;

        var elapsed = DateTime.UtcNow - state.LastRequestUtc.Value;
        var requiredDelay = TimeSpan.FromMilliseconds(sourceRuntimeDefinition.RateLimitMilliseconds.Value);
        if (elapsed >= requiredDelay)
            return;

        var remainingDelay = requiredDelay - elapsed;
        _appLogger.LogDebug("Fetch",
            "Rate limiting source '{0}' ({1}) engine '{2}' profile '{3}' for {4} ms before the next request.",
            sourceRuntimeDefinition.SourceName,
            sourceRuntimeDefinition.SourceId,
            sourceRuntimeDefinition.EngineType,
            ResolveProfileVersion(sourceRuntimeDefinition),
            (int)remainingDelay.TotalMilliseconds);
        await Task.Delay(remainingDelay, cancellationToken);
    }

    private void EnsureNotCoolingDown(SourceRuntimeDefinition sourceRuntimeDefinition, SourceRequestState state)
    {
        if (!state.CooldownUntilUtc.HasValue || state.CooldownUntilUtc.Value <= DateTime.UtcNow)
            return;

        var message =
            $"Source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{ResolveProfileVersion(sourceRuntimeDefinition)}' is cooling down until {state.CooldownUntilUtc.Value:O}.";
        _appLogger.LogWarning("Fetch", message);
        throw new InvalidOperationException(message);
    }

    private void RegisterFailure(SourceRuntimeDefinition sourceRuntimeDefinition, SourceRequestState state, string reason,
        string category = "transport")
    {
        state.ConsecutiveFailures++;

        if (state.ConsecutiveFailures < CooldownFailureThreshold)
            return;

        var cooldownMilliseconds = Math.Max(sourceRuntimeDefinition.RateLimitMilliseconds ?? 0,
            (int)DefaultCooldown.TotalMilliseconds);
        var cooldown = TimeSpan.FromMilliseconds(cooldownMilliseconds);
        state.CooldownUntilUtc = DateTime.UtcNow.Add(cooldown);

        _appLogger.LogWarning("Fetch",
            "Opened {0} cooldown for source '{1}' ({2}) engine '{3}' profile '{4}' after {5} consecutive failures. Cooldown: {6} ms. Reason: {7}",
            category,
            sourceRuntimeDefinition.SourceName,
            sourceRuntimeDefinition.SourceId,
            sourceRuntimeDefinition.EngineType,
            ResolveProfileVersion(sourceRuntimeDefinition),
            state.ConsecutiveFailures,
            cooldownMilliseconds,
            reason);
    }

    private static bool IsTransientException(Exception ex)
    {
        return ex is HttpRequestException or TaskCanceledException or TimeoutException;
    }

    private static bool IsTransientStatusCode(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.RequestTimeout || statusCode == (HttpStatusCode)429 || (int)statusCode >= 500;
    }

    private static string ResolveProfileVersion(SourceRuntimeDefinition sourceRuntimeDefinition)
    {
        return sourceRuntimeDefinition.ParsingProfile.GetValueOrDefault("ProfileVersion")
               ?? sourceRuntimeDefinition.RequestProfile.GetValueOrDefault("ProfileVersion")
               ?? "unknown";
    }

    private sealed class SourceRequestState
    {
        public SemaphoreSlim Gate { get; } = new(1, 1);
        public DateTime? LastRequestUtc { get; set; }
        public int ConsecutiveFailures { get; set; }
        public DateTime? CooldownUntilUtc { get; set; }
    }
}