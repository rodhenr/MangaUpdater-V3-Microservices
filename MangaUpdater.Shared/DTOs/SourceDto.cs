namespace MangaUpdater.Shared.DTOs;

public record SourceDto(
	int Id,
	string Name,
	string? Slug,
	string BaseUrl,
	bool IsEnabled,
	string EngineType,
	string RequestMode,
	bool RequiresBrowser,
	string? DefaultUserAgent,
	int? RateLimitMilliseconds,
	string? QueueName,
	bool SupportsPagination,
	DateTime Timestamp);