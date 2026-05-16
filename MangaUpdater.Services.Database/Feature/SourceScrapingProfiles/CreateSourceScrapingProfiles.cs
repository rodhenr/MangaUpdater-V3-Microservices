using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceScrapingProfiles;

public record CreateSourceScrapingProfileCommand(int SourceId, CreateSourceScrapingProfileRequest Profile) : IRequest;

public class CreateSourceScrapingProfileHandler : IRequestHandler<CreateSourceScrapingProfileCommand>
{
    private readonly AppDbContext _context;

    public CreateSourceScrapingProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateSourceScrapingProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        SourceConfigurationSupport.ValidateVersion(request.Profile.Version);

        var profile = new SourceScrapingProfile
        {
            SourceId = source.Id,
            IsActive = request.Profile.IsActive,
            Version = request.Profile.Version,
            ChapterNodesXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNodesXPath),
            ChapterUrlXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterUrlXPath),
            ChapterUrlAttribute = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterUrlAttribute),
            ChapterNumberXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberXPath),
            ChapterNumberAttribute = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberAttribute),
            ChapterNumberRegex = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberRegex),
            ChapterDateXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDateXPath),
            ChapterDateAttribute = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDateAttribute),
            ChapterDateRegex = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDateRegex),
            DateParseMode = SourceConfigurationSupport.NormalizeValue(request.Profile.DateParseMode),
            DateCulture = SourceConfigurationSupport.NormalizeValue(request.Profile.DateCulture),
            DateFormatPrimary = SourceConfigurationSupport.NormalizeValue(request.Profile.DateFormatPrimary),
            DateFormatSecondary = SourceConfigurationSupport.NormalizeValue(request.Profile.DateFormatSecondary),
            RelativeDateRegex = SourceConfigurationSupport.NormalizeValue(request.Profile.RelativeDateRegex),
            IgnoreTextContains1 = SourceConfigurationSupport.NormalizeValue(request.Profile.IgnoreTextContains1),
            IgnoreTextContains2 = SourceConfigurationSupport.NormalizeValue(request.Profile.IgnoreTextContains2),
            IgnoreTextContains3 = SourceConfigurationSupport.NormalizeValue(request.Profile.IgnoreTextContains3),
            UrlPrefix = SourceConfigurationSupport.NormalizeValue(request.Profile.UrlPrefix),
            UrlJoinMode = SourceConfigurationSupport.NormalizeValue(request.Profile.UrlJoinMode),
            DeduplicationKeyMode = SourceConfigurationSupport.NormalizeValue(request.Profile.DeduplicationKeyMode),
            ChapterSortMode = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterSortMode),
            PaginationNextPageXPath = SourceConfigurationSupport.NormalizeValue(request.Profile.PaginationNextPageXPath),
            PaginationUrlTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.PaginationUrlTemplate),
            ResultLimit = request.Profile.ResultLimit,
            Timestamp = DateTime.UtcNow
        };

        source.ScrapingProfiles.Add(profile);
        SourceConfigurationSupport.ActivateScrapingProfile(source, profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}