using MangaUpdater.Shared.Models;

namespace MangaUpdater.Shared.DTOs;

public record SourceRequestProfileDto(
    int Id,
    int SourceId,
    bool IsActive,
    int Version,
    string Method,
    string? UrlTemplate,
    string? HeadersJson,
    int? TimeoutSeconds,
    bool UseCookies,
    string? AcceptLanguage,
    string? Referrer,
    DateTime Timestamp);

public record SourceApiProfileDto(
    int Id,
    int SourceId,
    bool IsActive,
    int Version,
    string HttpMethod,
    string? EndpointTemplate,
    string? DataRootPath,
    string? ChapterNumberPath,
    string? ChapterDatePath,
    string? ChapterUrlPath,
    string? ResultUrlTemplate,
    string? PaginationMode,
    string? OffsetParameterName,
    string? LimitParameterName,
    string? NextPagePath,
    int? ResultLimit,
    DateTime Timestamp);

public record SourceScrapingProfileDto(
    int Id,
    int SourceId,
    bool IsActive,
    int Version,
    string? ChapterNodesXPath,
    string? ChapterUrlXPath,
    string? ChapterUrlAttribute,
    string? ChapterNumberXPath,
    string? ChapterNumberAttribute,
    string? ChapterNumberRegex,
    string? ChapterDateXPath,
    string? ChapterDateAttribute,
    string? ChapterDateRegex,
    string? DateParseMode,
    string? DateCulture,
    string? DateFormatPrimary,
    string? DateFormatSecondary,
    string? RelativeDateRegex,
    string? IgnoreTextContains1,
    string? IgnoreTextContains2,
    string? IgnoreTextContains3,
    string? UrlPrefix,
    string? UrlJoinMode,
    string? DeduplicationKeyMode,
    string? ChapterSortMode,
    string? PaginationNextPageXPath,
    string? PaginationUrlTemplate,
    int? ResultLimit,
    DateTime Timestamp);

public record SourceProfileValidationResultDto(
    string EngineType,
    string RequestUrl,
    IReadOnlyCollection<GenericChapterCandidate> Preview,
    IReadOnlyCollection<string> Warnings,
    IReadOnlyCollection<string> Diagnostics);