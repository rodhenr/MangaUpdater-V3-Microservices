using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.DTOs;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceScrapingProfiles;

public record GetSourceScrapingProfilesQuery(int SourceId) : IRequest<List<SourceScrapingProfileDto>>;

public class GetSourceScrapingProfilesHandler : IRequestHandler<GetSourceScrapingProfilesQuery, List<SourceScrapingProfileDto>>
{
    private readonly AppDbContext _context;

    public GetSourceScrapingProfilesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SourceScrapingProfileDto>> Handle(GetSourceScrapingProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        return source.ScrapingProfiles
            .OrderByDescending(profile => profile.Version)
            .ThenByDescending(profile => profile.Id)
            .Select(profile => new SourceScrapingProfileDto(profile.Id, profile.SourceId, profile.IsActive,
                profile.Version, profile.ChapterNodesXPath, profile.ChapterUrlXPath, profile.ChapterUrlAttribute,
                profile.ChapterNumberXPath, profile.ChapterNumberAttribute, profile.ChapterNumberRegex,
                profile.ChapterDateXPath, profile.ChapterDateAttribute, profile.ChapterDateRegex,
                profile.DateParseMode, profile.DateCulture, profile.DateFormatPrimary, profile.DateFormatSecondary,
                profile.RelativeDateRegex, profile.IgnoreTextContains1, profile.IgnoreTextContains2,
                profile.IgnoreTextContains3, profile.UrlPrefix, profile.UrlJoinMode, profile.DeduplicationKeyMode,
                profile.ChapterSortMode, profile.PaginationNextPageXPath, profile.PaginationUrlTemplate,
                profile.ResultLimit, profile.Timestamp))
            .ToList();
    }
}