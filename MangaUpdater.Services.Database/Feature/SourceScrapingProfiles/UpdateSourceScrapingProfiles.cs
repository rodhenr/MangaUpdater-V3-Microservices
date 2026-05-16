using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceScrapingProfiles;

public record UpdateSourceScrapingProfileCommand(int SourceId, int ProfileId, UpdateSourceScrapingProfileRequest Profile) : IRequest;

public class UpdateSourceScrapingProfileHandler : IRequestHandler<UpdateSourceScrapingProfileCommand>
{
    private readonly AppDbContext _context;

    public UpdateSourceScrapingProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSourceScrapingProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var profile = source.ScrapingProfiles.FirstOrDefault(candidate => candidate.Id == request.ProfileId)
                      ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "Scraping profile not found");

        SourceConfigurationSupport.ValidateVersion(request.Profile.Version);
        profile.IsActive = request.Profile.IsActive;
        profile.Version = request.Profile.Version;
        profile.ChapterNodesXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNodesXPath);
        profile.ChapterUrlXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterUrlXPath);
        profile.ChapterUrlAttribute = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterUrlAttribute);
        profile.ChapterNumberXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberXPath);
        profile.ChapterNumberAttribute = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberAttribute);
        profile.ChapterNumberRegex = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberRegex);
        profile.ChapterDateXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDateXPath);
        profile.ChapterDateAttribute = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDateAttribute);
        profile.ChapterDateRegex = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDateRegex);
        profile.DateParseMode = SourceConfigurationSupport.NormalizeValue(request.Profile.DateParseMode);
        profile.DateCulture = SourceConfigurationSupport.NormalizeValue(request.Profile.DateCulture);
        profile.DateFormatPrimary = SourceConfigurationSupport.NormalizeValue(request.Profile.DateFormatPrimary);
        profile.DateFormatSecondary = SourceConfigurationSupport.NormalizeValue(request.Profile.DateFormatSecondary);
        profile.RelativeDateRegex = SourceConfigurationSupport.NormalizeValue(request.Profile.RelativeDateRegex);
        profile.IgnoreTextContains1 = SourceConfigurationSupport.NormalizeValue(request.Profile.IgnoreTextContains1);
        profile.IgnoreTextContains2 = SourceConfigurationSupport.NormalizeValue(request.Profile.IgnoreTextContains2);
        profile.IgnoreTextContains3 = SourceConfigurationSupport.NormalizeValue(request.Profile.IgnoreTextContains3);
        profile.UrlPrefix = SourceConfigurationSupport.NormalizeValue(request.Profile.UrlPrefix);
        profile.UrlJoinMode = SourceConfigurationSupport.NormalizeValue(request.Profile.UrlJoinMode);
        profile.DeduplicationKeyMode = SourceConfigurationSupport.NormalizeValue(request.Profile.DeduplicationKeyMode);
        profile.ChapterSortMode = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterSortMode);
        profile.PaginationNextPageXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.PaginationNextPageXPath);
        profile.PaginationUrlTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.PaginationUrlTemplate);
        profile.ResultLimit = request.Profile.ResultLimit;
        profile.Timestamp = DateTime.UtcNow;

        SourceConfigurationSupport.ActivateScrapingProfile(source, profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}