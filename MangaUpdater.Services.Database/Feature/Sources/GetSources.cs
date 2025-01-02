using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record GetSourcesQuery : IRequest<List<SourceDto>>;

public class GetSourcesHandler : IRequestHandler<GetSourcesQuery, List<SourceDto>>
{
    private readonly AppDbContext _context;

    public GetSourcesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SourceDto>> Handle(GetSourcesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Sources
            .Select(x => new SourceDto(
                x.Id,
                x.Name,
                x.BaseUrl
            ))
            .ToListAsync(cancellationToken);
    }
}