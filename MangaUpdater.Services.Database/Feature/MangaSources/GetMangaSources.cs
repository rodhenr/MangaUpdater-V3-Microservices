using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.MangaSources;

public record GetMangaSourcesQuery(int PageNumber, int PageSize) : IRequest<PagedResultDto<MangaSourceDto>>;

public class GetMangaSourcesHandler : IRequestHandler<GetMangaSourcesQuery, PagedResultDto<MangaSourceDto>>
{
    private readonly AppDbContext _context;

    public GetMangaSourcesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<MangaSourceDto>> Handle(GetMangaSourcesQuery request, CancellationToken cancellationToken)
    {
        var mangaSourcesCount = await _context.MangaSources.CountAsync(cancellationToken);
        
        var mangaSources = await _context.MangaSources
            .OrderByDescending(x => x.Timestamp)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new MangaSourceDto(
                x.Id,
                x.MangaId,
                x.Manga.TitleEnglish,
                x.SourceId,
                x.Source.Name,
                x.Url,
                x.AdditionalInfo
            ))
            .ToListAsync(cancellationToken);
        
        return new PagedResultDto<MangaSourceDto>(mangaSources, mangaSourcesCount, request.PageNumber,request.PageSize,(int)Math.Ceiling((double)mangaSourcesCount / request.PageSize));
    }
}