namespace MangaUpdater.Shared.Models;

public record SourceRuntimeDefinition(
    int SourceId,
    string? SourceSlug,
    string SourceName,
    string SourceBaseUrl,
    string EngineType,
    string RequestMode,
    bool IsEnabled,
    bool RequiresBrowser,
    string? DefaultUserAgent,
    int? RateLimitMilliseconds,
    string? QueueName,
    bool SupportsPagination,
    IReadOnlyDictionary<string, string?> RequestProfile,
    IReadOnlyDictionary<string, string?> ParsingProfile);