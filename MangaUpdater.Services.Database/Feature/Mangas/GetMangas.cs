using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record GetMangasQuery : IRequest<List<MangaDto>>;

public class GetMangasHandler : IRequestHandler<GetMangasQuery, List<MangaDto>>
{
    private readonly AppDbContext _context;

    public GetMangasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MangaDto>> Handle(GetMangasQuery request, CancellationToken cancellationToken)
    {
        return await _context.Mangas
            .Select(x => new MangaDto(
                x.Id,
                x.MyAnimeListId,
                x.AniListId,
                x.TitleRomaji,
                x.TitleEnglish
            ))
            .ToListAsync(cancellationToken);
    }
}