using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record GetMangasQuery(int PageNumber, int PageSize) : IRequest<PagedResultDto<MangaDto>>;

public class GetMangasHandler : IRequestHandler<GetMangasQuery, PagedResultDto<MangaDto>>
{
    private readonly AppDbContext _context;

    public GetMangasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<MangaDto>> Handle(GetMangasQuery request, CancellationToken cancellationToken)
    {
        var mangasCount = await _context.Mangas.CountAsync(cancellationToken);
        
        var mangas = await _context.Mangas
            .OrderByDescending(x => x.Timestamp)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new MangaDto(
                x.Id,
                x.MyAnimeListId,
                x.AniListId,
                x.TitleRomaji,
                x.TitleEnglish,
                x.CoverUrl,
                x.Timestamp
            ))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<MangaDto>(mangas, mangasCount, request.PageNumber,request.PageSize,(int)Math.Ceiling((double)mangasCount / request.PageSize));
    }
}