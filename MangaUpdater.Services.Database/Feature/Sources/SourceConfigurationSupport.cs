using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Sources;

internal static class SourceConfigurationSupport
{
    public static async Task<Source> LoadSourceGraphAsync(AppDbContext context, int sourceId, CancellationToken cancellationToken)
    {
        var source = await context.Sources
            .Include(candidate => candidate.RequestProfiles)
            .Include(candidate => candidate.ApiProfiles)
            .Include(candidate => candidate.ScrapingProfiles)
            .FirstOrDefaultAsync(candidate => candidate.Id == sourceId, cancellationToken);

        return source ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "Source not found");
    }

    public static string? NormalizeValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static string NormalizeRequiredValue(string? value, string fieldName)
    {
        var normalized = NormalizeValue(value);
        return !string.IsNullOrWhiteSpace(normalized) 
            ? normalized 
            : throw new HttpResponseException(HttpStatusCode.BadRequest, $"{fieldName} is required");
    }

    public static void ValidateVersion(int version)
    {
        if (version <= 0)
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Version must be greater than zero");
    }

    public static void ActivateRequestProfile(Source source, SourceRequestProfile profile)
    {
        if (!profile.IsActive)
            return;

        foreach (var candidate in source.RequestProfiles.Where(candidate => candidate.Id != profile.Id))
            candidate.IsActive = false;
    }

    public static void ActivateApiProfile(Source source, SourceApiProfile profile)
    {
        if (!profile.IsActive)
            return;

        foreach (var candidate in source.ApiProfiles.Where(candidate => candidate.Id != profile.Id))
            candidate.IsActive = false;
    }

    public static void ActivateScrapingProfile(Source source, SourceScrapingProfile profile)
    {
        if (!profile.IsActive)
            return;

        foreach (var candidate in source.ScrapingProfiles.Where(candidate => candidate.Id != profile.Id))
            candidate.IsActive = false;
    }

    public static void ValidateSourceConfigurationOrThrow(Source source)
    {
        var errors = ValidateSourceConfiguration(source);
        if (errors.Count == 0)
            return;

        throw new HttpResponseException(HttpStatusCode.BadRequest, string.Join(" ", errors));
    }

    private static IReadOnlyCollection<string> ValidateSourceConfiguration(Source source)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(source.Name))
            errors.Add("Source name is required.");

        if (string.IsNullOrWhiteSpace(source.BaseUrl))
            errors.Add("Source base URL is required.");

        if (string.IsNullOrWhiteSpace(source.EngineType))
            errors.Add("Source engine type is required.");

        if (string.IsNullOrWhiteSpace(source.RequestMode))
            errors.Add("Source request mode is required.");

        ValidateSingleActive(errors, source.RequestProfiles.Count(candidate => candidate.IsActive), "request");
        ValidateSingleActive(errors, source.ApiProfiles.Count(candidate => candidate.IsActive), "API");
        ValidateSingleActive(errors, source.ScrapingProfiles.Count(candidate => candidate.IsActive), "scraping");

        if (!source.IsEnabled)
            return errors;

        if (string.Equals(source.EngineType, "HtmlXPath", StringComparison.OrdinalIgnoreCase))
        {
            var scrapingProfile = source.ScrapingProfiles
                .Where(candidate => candidate.IsActive)
                .OrderByDescending(candidate => candidate.Version)
                .ThenByDescending(candidate => candidate.Id)
                .FirstOrDefault();

            if (scrapingProfile is null)
            {
                errors.Add("Enabled HtmlXPath sources require one active scraping profile.");
            }
            else if (string.IsNullOrWhiteSpace(scrapingProfile.ChapterNodesXPath))
            {
                errors.Add("Active HtmlXPath scraping profile must define ChapterNodesXPath.");
            }
        }

        if (!string.Equals(source.EngineType, "JsonApi", StringComparison.OrdinalIgnoreCase)) return errors;
        
        var apiProfile = source.ApiProfiles
            .Where(candidate => candidate.IsActive)
            .OrderByDescending(candidate => candidate.Version)
            .ThenByDescending(candidate => candidate.Id)
            .FirstOrDefault();

        if (apiProfile is null)
        {
            errors.Add("Enabled JsonApi sources require one active API profile.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(apiProfile.DataRootPath))
                errors.Add("Active JsonApi profile must define DataRootPath.");

            if (string.IsNullOrWhiteSpace(apiProfile.ChapterNumberPath))
                errors.Add("Active JsonApi profile must define ChapterNumberPath.");
        }

        return errors;
    }

    public static SourceRequestProfile? SelectRequestProfile(Source source, int? profileId)
    {
        return profileId.HasValue
            ? source.RequestProfiles.FirstOrDefault(candidate => candidate.Id == profileId.Value)
            : source.RequestProfiles
                .Where(candidate => candidate.IsActive)
                .OrderByDescending(candidate => candidate.Version)
                .ThenByDescending(candidate => candidate.Id)
                .FirstOrDefault();
    }

    public static SourceApiProfile? SelectApiProfile(Source source, int? profileId)
    {
        return profileId.HasValue
            ? source.ApiProfiles.FirstOrDefault(candidate => candidate.Id == profileId.Value)
            : source.ApiProfiles
                .Where(candidate => candidate.IsActive)
                .OrderByDescending(candidate => candidate.Version)
                .ThenByDescending(candidate => candidate.Id)
                .FirstOrDefault();
    }

    public static SourceScrapingProfile? SelectScrapingProfile(Source source, int? profileId)
    {
        return profileId.HasValue
            ? source.ScrapingProfiles.FirstOrDefault(candidate => candidate.Id == profileId.Value)
            : source.ScrapingProfiles
                .Where(candidate => candidate.IsActive)
                .OrderByDescending(candidate => candidate.Version)
                .ThenByDescending(candidate => candidate.Id)
                .FirstOrDefault();
    }

    private static void ValidateSingleActive(List<string> errors, int activeCount, string profileLabel)
    {
        if (activeCount > 1)
            errors.Add($"Only one active {profileLabel} profile is allowed per source.");
    }
}