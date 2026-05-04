namespace MangaUpdater.Shared.Models;

public record CreateSourceRequestProfileRequest
{
    public bool IsActive { get; init; } = true;
    public int Version { get; init; } = 1;
    public string Method { get; init; } = "GET";
    public string? UrlTemplate { get; init; }
    public string? HeadersJson { get; init; }
    public int? TimeoutSeconds { get; init; }
    public bool UseCookies { get; init; }
    public string? AcceptLanguage { get; init; }
    public string? Referrer { get; init; }
}

public sealed record UpdateSourceRequestProfileRequest : CreateSourceRequestProfileRequest;

public record CreateSourceApiProfileRequest
{
    public bool IsActive { get; init; } = true;
    public int Version { get; init; } = 1;
    public string HttpMethod { get; init; } = "GET";
    public string? EndpointTemplate { get; init; }
    public string? DataRootPath { get; init; }
    public string? ChapterNumberPath { get; init; }
    public string? ChapterDatePath { get; init; }
    public string? ChapterUrlPath { get; init; }
    public string? ResultUrlTemplate { get; init; }
    public string? PaginationMode { get; init; }
    public string? OffsetParameterName { get; init; }
    public string? LimitParameterName { get; init; }
    public string? NextPagePath { get; init; }
    public int? ResultLimit { get; init; }
}

public sealed record UpdateSourceApiProfileRequest : CreateSourceApiProfileRequest;

public record CreateSourceScrapingProfileRequest
{
    public bool IsActive { get; init; } = true;
    public int Version { get; init; } = 1;
    public string? ChapterNodesXPath { get; init; }
    public string? ChapterUrlXPath { get; init; }
    public string? ChapterUrlAttribute { get; init; }
    public string? ChapterNumberXPath { get; init; }
    public string? ChapterNumberAttribute { get; init; }
    public string? ChapterNumberRegex { get; init; }
    public string? ChapterDateXPath { get; init; }
    public string? ChapterDateAttribute { get; init; }
    public string? ChapterDateRegex { get; init; }
    public string? DateParseMode { get; init; }
    public string? DateCulture { get; init; }
    public string? DateFormatPrimary { get; init; }
    public string? DateFormatSecondary { get; init; }
    public string? RelativeDateRegex { get; init; }
    public string? IgnoreTextContains1 { get; init; }
    public string? IgnoreTextContains2 { get; init; }
    public string? IgnoreTextContains3 { get; init; }
    public string? UrlPrefix { get; init; }
    public string? UrlJoinMode { get; init; }
    public string? DeduplicationKeyMode { get; init; }
    public string? ChapterSortMode { get; init; }
    public string? PaginationNextPageXPath { get; init; }
    public string? PaginationUrlTemplate { get; init; }
    public int? ResultLimit { get; init; }
}

public sealed record UpdateSourceScrapingProfileRequest : CreateSourceScrapingProfileRequest;

public sealed record ValidateSourceProfileRequest
{
    public required string TargetUrl { get; init; }
    public int? RequestProfileId { get; init; }
    public int? ApiProfileId { get; init; }
    public int? ScrapingProfileId { get; init; }
    public int PreviewLimit { get; init; } = 20;
}