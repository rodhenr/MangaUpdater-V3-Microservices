using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.DTOs;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceApiProfiles;

public record GetSourceApiProfilesQuery(int SourceId) : IRequest<List<SourceApiProfileDto>>;

public class GetSourceApiProfilesHandler : IRequestHandler<GetSourceApiProfilesQuery, List<SourceApiProfileDto>>
{
    private readonly AppDbContext _context;

    public GetSourceApiProfilesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SourceApiProfileDto>> Handle(GetSourceApiProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        return source.ApiProfiles
            .OrderByDescending(profile => profile.Version)
            .ThenByDescending(profile => profile.Id)
            .Select(profile => new SourceApiProfileDto(profile.Id, profile.SourceId, profile.IsActive, profile.Version,
                profile.HttpMethod, profile.EndpointTemplate, profile.DataRootPath, profile.ChapterNumberPath,
                profile.ChapterDatePath, profile.ChapterUrlPath, profile.ResultUrlTemplate, profile.PaginationMode,
                profile.OffsetParameterName, profile.LimitParameterName, profile.NextPagePath, profile.ResultLimit,
                profile.Timestamp))
            .ToList();
    }
}