namespace MangaUpdater.Shared.Models;

public sealed record CreateSourceRequest
{
	public required string Name { get; init; }
	public string? Slug { get; init; }
	public string? BaseUrl { get; init; }
	public string? Url { get; init; }
	public bool IsEnabled { get; init; } = true;
	public string EngineType { get; init; } = "HtmlXPath";
	public string RequestMode { get; init; } = "HttpGet";
	public bool RequiresBrowser { get; init; }
	public string? DefaultUserAgent { get; init; }
	public int? RateLimitMilliseconds { get; init; }
	public string? QueueName { get; init; }
	public bool SupportsPagination { get; init; }

	public string ResolveBaseUrl() => BaseUrl ?? Url ?? string.Empty;
}

public sealed record UpdateSourceRequest
{
	public required string Name { get; init; }
	public string? Slug { get; init; }
	public string? BaseUrl { get; init; }
	public string? Url { get; init; }
	public bool IsEnabled { get; init; } = true;
	public string EngineType { get; init; } = "HtmlXPath";
	public string RequestMode { get; init; } = "HttpGet";
	public bool RequiresBrowser { get; init; }
	public string? DefaultUserAgent { get; init; }
	public int? RateLimitMilliseconds { get; init; }
	public string? QueueName { get; init; }
	public bool SupportsPagination { get; init; }

	public string ResolveBaseUrl() => BaseUrl ?? Url ?? string.Empty;
}