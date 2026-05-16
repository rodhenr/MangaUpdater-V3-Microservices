using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.DTOs;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceRequest;

public record GetSourceRequestProfilesQuery(int SourceId) : IRequest<List<SourceRequestProfileDto>>;

public class GetSourceRequestProfilesHandler : IRequestHandler<GetSourceRequestProfilesQuery, List<SourceRequestProfileDto>>
{
    private readonly AppDbContext _context;

    public GetSourceRequestProfilesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SourceRequestProfileDto>> Handle(GetSourceRequestProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        return source.RequestProfiles
            .OrderByDescending(profile => profile.Version)
            .ThenByDescending(profile => profile.Id)
            .Select(Map)
            .ToList();
    }

    private static SourceRequestProfileDto Map(SourceRequestProfile profile)
    {
        return new SourceRequestProfileDto(profile.Id, profile.SourceId, profile.IsActive, profile.Version, profile.Method,
            profile.UrlTemplate, profile.HeadersJson, profile.TimeoutSeconds, profile.UseCookies, profile.AcceptLanguage,
            profile.Referrer, profile.Timestamp);
    }
}