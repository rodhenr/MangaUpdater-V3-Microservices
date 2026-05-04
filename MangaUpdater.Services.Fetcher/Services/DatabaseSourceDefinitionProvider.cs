using System.Text.Json;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MangaUpdater.Services.Fetcher.Services;

public class DatabaseSourceDefinitionProvider : ISourceDefinitionProvider
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAppLogger _appLogger;

    public DatabaseSourceDefinitionProvider(IMemoryCache memoryCache, IServiceScopeFactory serviceScopeFactory,
        IAppLogger appLogger)
    {
        _memoryCache = memoryCache;
        _serviceScopeFactory = serviceScopeFactory;
        _appLogger = appLogger;
    }

    public async Task<SourceRuntimeDefinition> GetSourceDefinitionAsync(ChapterQueueMessageDto request,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"source-definition:{request.SourceId}";
        if (_memoryCache.TryGetValue<SourceRuntimeDefinition>(cacheKey, out var cachedDefinition))
        {
            _appLogger.LogDebug("FetchConfig",
                "Source definition cache hit for source '{0}' ({1}) engine '{2}' profile '{3}'. TTL strategy: 5 minute in-memory cache without explicit remote invalidation.",
                cachedDefinition!.SourceName,
                cachedDefinition.SourceId,
                cachedDefinition.EngineType,
                ResolveProfileVersion(cachedDefinition));
            return cachedDefinition;
        }

        _appLogger.LogDebug("FetchConfig",
            "Source definition cache miss for source id '{0}'. Loading source configuration from database.",
            request.SourceId);

        var resolvedDefinition = await LoadFromDatabaseAsync(request, cancellationToken)
                                 ?? throw new InvalidOperationException(
                                     $"No database-backed source definition could be resolved for source id '{request.SourceId}'. Apply the latest database migrations and verify the source profiles.");

        _memoryCache.Set(cacheKey, resolvedDefinition, CacheDuration);
        _appLogger.LogDebug("FetchConfig",
            "Cached source definition for source '{0}' ({1}) engine '{2}' profile '{3}' with TTL of {4} minutes.",
            resolvedDefinition.SourceName,
            resolvedDefinition.SourceId,
            resolvedDefinition.EngineType,
            ResolveProfileVersion(resolvedDefinition),
            (int)CacheDuration.TotalMinutes);
        return resolvedDefinition;
    }

    private async Task<SourceRuntimeDefinition?> LoadFromDatabaseAsync(ChapterQueueMessageDto request,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var source = await dbContext.Sources
            .AsNoTracking()
            .Include(candidate => candidate.RequestProfiles)
            .Include(candidate => candidate.ApiProfiles)
            .Include(candidate => candidate.ScrapingProfiles)
            .FirstOrDefaultAsync(candidate => candidate.Id == request.SourceId, cancellationToken);

        if (source is null)
        {
            _appLogger.LogError("FetchConfig",
                $"No source configuration was found in the database for source id '{request.SourceId}'.");
            return null;
        }

        if (!source.IsEnabled)
        {
            _appLogger.LogWarning("FetchConfig",
                "Source '{0}' ({1}) engine '{2}' is disabled and cannot be loaded for scraping.",
                source.Name,
                source.Id,
                source.EngineType);
            throw new InvalidOperationException($"Source '{source.Name}' ({source.Id}) is disabled.");
        }

        var requestProfile = ResolveRequestProfile(source);
        var parsingProfile = ResolveParsingProfile(source);

        if ((string.Equals(source.EngineType, "HtmlXPath", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(source.EngineType, "JsonApi", StringComparison.OrdinalIgnoreCase)) &&
            parsingProfile.Count == 0)
        {
            _appLogger.LogWarning("FetchConfig",
                "No active parsing profile found in database for source '{0}' ({1}) engine '{2}'. Request profiles: {3}, API profiles: {4}, scraping profiles: {5}.",
                source.Name,
                source.Id,
                source.EngineType,
                source.RequestProfiles.Count,
                source.ApiProfiles.Count,
                source.ScrapingProfiles.Count);
            return null;
        }

        var definition = new SourceRuntimeDefinition(
            source.Id,
            source.Slug,
            source.Name,
            source.BaseUrl,
            source.EngineType,
            source.RequestMode,
            source.IsEnabled,
            source.RequiresBrowser,
            source.DefaultUserAgent,
            source.RateLimitMilliseconds,
            source.QueueName,
            source.SupportsPagination,
            requestProfile,
            parsingProfile);

        _appLogger.LogDebug("FetchConfig",
            "Resolved source definition for source '{0}' ({1}) engine '{2}' profile '{3}'. Request profile version '{4}'.",
            definition.SourceName,
            definition.SourceId,
            definition.EngineType,
            ResolveProfileVersion(definition),
            definition.RequestProfile.GetValueOrDefault("ProfileVersion") ?? "unknown");

        return definition;
    }

    private static IReadOnlyDictionary<string, string?> ResolveRequestProfile(Source source)
    {
        var requestProfile = source.RequestProfiles
            .Where(candidate => candidate.IsActive)
            .OrderByDescending(candidate => candidate.Version)
            .ThenByDescending(candidate => candidate.Id)
            .FirstOrDefault();

        if (requestProfile is null)
        {
            var apiFallbackProfile = source.ApiProfiles
                .Where(candidate => candidate.IsActive)
                .OrderByDescending(candidate => candidate.Version)
                .ThenByDescending(candidate => candidate.Id)
                .FirstOrDefault();

            return apiFallbackProfile is null
                ? new Dictionary<string, string?>()
                : new Dictionary<string, string?>
                {
                    ["UrlTemplate"] = apiFallbackProfile.EndpointTemplate
                };
        }

        var values = new Dictionary<string, string?>
        {
            ["ProfileVersion"] = requestProfile.Version.ToString(),
            ["Method"] = requestProfile.Method,
            ["UrlTemplate"] = requestProfile.UrlTemplate,
            ["HeadersJson"] = requestProfile.HeadersJson,
            ["TimeoutSeconds"] = requestProfile.TimeoutSeconds?.ToString(),
            ["UseCookies"] = requestProfile.UseCookies.ToString(),
            ["AcceptLanguage"] = requestProfile.AcceptLanguage,
            ["Referrer"] = requestProfile.Referrer
        };

        if (string.IsNullOrWhiteSpace(values["UrlTemplate"]))
        {
            var apiProfile = source.ApiProfiles
                .Where(candidate => candidate.IsActive)
                .OrderByDescending(candidate => candidate.Version)
                .ThenByDescending(candidate => candidate.Id)
                .FirstOrDefault();
            values["UrlTemplate"] = apiProfile?.EndpointTemplate;
        }

        var headers = TryReadHeaders(requestProfile.HeadersJson);
        foreach (var header in headers)
            values[$"Header:{header.Key}"] = header.Value;

        return values;
    }

    private static IReadOnlyDictionary<string, string?> ResolveParsingProfile(Source source)
    {
        if (string.Equals(source.EngineType, "HtmlXPath", StringComparison.OrdinalIgnoreCase))
        {
            var scrapingProfile = source.ScrapingProfiles
                .Where(candidate => candidate.IsActive)
                .OrderByDescending(candidate => candidate.Version)
                .ThenByDescending(candidate => candidate.Id)
                .FirstOrDefault();

            return scrapingProfile is null
                ? new Dictionary<string, string?>()
                : BuildScrapingParsingProfile(scrapingProfile);
        }

        if (!string.Equals(source.EngineType, "JsonApi", StringComparison.OrdinalIgnoreCase))
            return new Dictionary<string, string?>();
        
        var apiProfile = source.ApiProfiles
            .Where(candidate => candidate.IsActive)
            .OrderByDescending(candidate => candidate.Version)
            .ThenByDescending(candidate => candidate.Id)
            .FirstOrDefault();

        return apiProfile is null
            ? new Dictionary<string, string?>()
            : BuildApiParsingProfile(apiProfile);
    }

    private static IReadOnlyDictionary<string, string?> BuildScrapingParsingProfile(SourceScrapingProfile scrapingProfile)
    {
        return new Dictionary<string, string?>
        {
            ["ProfileVersion"] = scrapingProfile.Version.ToString(),
            ["ChapterNodesXPath"] = scrapingProfile.ChapterNodesXPath,
            ["ChapterUrlXPath"] = scrapingProfile.ChapterUrlXPath,
            ["ChapterUrlAttribute"] = scrapingProfile.ChapterUrlAttribute,
            ["ChapterNumberXPath"] = scrapingProfile.ChapterNumberXPath,
            ["ChapterNumberAttribute"] = scrapingProfile.ChapterNumberAttribute,
            ["ChapterNumberRegex"] = scrapingProfile.ChapterNumberRegex,
            ["ChapterDateXPath"] = scrapingProfile.ChapterDateXPath,
            ["ChapterDateAttribute"] = scrapingProfile.ChapterDateAttribute,
            ["ChapterDateRegex"] = scrapingProfile.ChapterDateRegex,
            ["DateParseMode"] = scrapingProfile.DateParseMode,
            ["DateCulture"] = scrapingProfile.DateCulture,
            ["DateFormatPrimary"] = scrapingProfile.DateFormatPrimary,
            ["DateFormatSecondary"] = scrapingProfile.DateFormatSecondary,
            ["RelativeDateRegex"] = scrapingProfile.RelativeDateRegex,
            ["IgnoreTextContains1"] = scrapingProfile.IgnoreTextContains1,
            ["IgnoreTextContains2"] = scrapingProfile.IgnoreTextContains2,
            ["IgnoreTextContains3"] = scrapingProfile.IgnoreTextContains3,
            ["UrlPrefix"] = scrapingProfile.UrlPrefix,
            ["UrlJoinMode"] = scrapingProfile.UrlJoinMode,
            ["DeduplicationKeyMode"] = scrapingProfile.DeduplicationKeyMode,
            ["ChapterSortMode"] = scrapingProfile.ChapterSortMode,
            ["PaginationNextPageXPath"] = scrapingProfile.PaginationNextPageXPath,
            ["PaginationUrlTemplate"] = scrapingProfile.PaginationUrlTemplate,
            ["ResultLimit"] = scrapingProfile.ResultLimit?.ToString()
        };
    }

    private static IReadOnlyDictionary<string, string?> BuildApiParsingProfile(SourceApiProfile apiProfile)
    {
        return new Dictionary<string, string?>
        {
            ["ProfileVersion"] = apiProfile.Version.ToString(),
            ["DataRootPath"] = apiProfile.DataRootPath,
            ["ChapterNumberPath"] = apiProfile.ChapterNumberPath,
            ["ChapterDatePath"] = apiProfile.ChapterDatePath,
            ["ChapterUrlPath"] = apiProfile.ChapterUrlPath,
            ["ResultUrlTemplate"] = apiProfile.ResultUrlTemplate,
            ["PaginationMode"] = apiProfile.PaginationMode,
            ["OffsetParameterName"] = apiProfile.OffsetParameterName,
            ["LimitParameterName"] = apiProfile.LimitParameterName,
            ["NextPagePath"] = apiProfile.NextPagePath,
            ["ResultLimit"] = apiProfile.ResultLimit?.ToString()
        };
    }

    private static IReadOnlyDictionary<string, string?> TryReadHeaders(string? headersJson)
    {
        if (string.IsNullOrWhiteSpace(headersJson))
            return new Dictionary<string, string?>();

        try
        {
            var headers = JsonSerializer.Deserialize<Dictionary<string, string?>>(headersJson);
            return headers ?? new Dictionary<string, string?>();
        }
        catch
        {
            return new Dictionary<string, string?>();
        }
    }

    private static string ResolveProfileVersion(SourceRuntimeDefinition sourceRuntimeDefinition)
    {
        return sourceRuntimeDefinition.ParsingProfile.GetValueOrDefault("ProfileVersion")
               ?? sourceRuntimeDefinition.RequestProfile.GetValueOrDefault("ProfileVersion")
               ?? "unknown";
    }
}