using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record GetSourceScrapingProfilesQuery(int SourceId) : IRequest<List<SourceScrapingProfileDto>>;
public record CreateSourceScrapingProfileCommand(int SourceId, CreateSourceScrapingProfileRequest Profile) : IRequest;
public record UpdateSourceScrapingProfileCommand(int SourceId, int ProfileId, UpdateSourceScrapingProfileRequest Profile) : IRequest;
public record DeleteSourceScrapingProfileCommand(int SourceId, int ProfileId) : IRequest;

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

public class DeleteSourceScrapingProfileHandler : IRequestHandler<DeleteSourceScrapingProfileCommand>
{
    private readonly AppDbContext _context;

    public DeleteSourceScrapingProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSourceScrapingProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var profile = source.ScrapingProfiles.FirstOrDefault(candidate => candidate.Id == request.ProfileId)
                      ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "Scraping profile not found");

        _context.SourceScrapingProfiles.Remove(profile);
        source.ScrapingProfiles.Remove(profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}