using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record GetSourcesQuery(int PageNumber, int PageSize) : IRequest<PagedResultDto<SourceDto>>;

public class GetSourcesHandler : IRequestHandler<GetSourcesQuery, PagedResultDto<SourceDto>>
{
    private readonly AppDbContext _context;

    public GetSourcesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<SourceDto>> Handle(GetSourcesQuery request, CancellationToken cancellationToken)
    { 
        var sourcesCount = await _context.Sources.CountAsync(cancellationToken);

        var sources = await _context.Sources
            .OrderByDescending(x => x.Timestamp)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SourceDto(
                x.Id,
                x.Name,
                x.BaseUrl
            ))
            .ToListAsync(cancellationToken);
        
        return new PagedResultDto<SourceDto>(sources, sourcesCount, request.PageNumber,request.PageSize,(int)Math.Ceiling((double)sourcesCount / request.PageSize));
    }
}