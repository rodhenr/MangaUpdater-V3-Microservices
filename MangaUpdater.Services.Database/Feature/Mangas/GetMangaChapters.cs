using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record GetMangaChaptersQuery(int MyAnimeListId) : IRequest<MangaChaptersDto?>;

public class GetMangaChaptersHandler : IRequestHandler<GetMangaChaptersQuery, MangaChaptersDto?>
{
    private readonly AppDbContext _context;

    public GetMangaChaptersHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MangaChaptersDto?> Handle(GetMangaChaptersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Mangas
            .Where(x => x.MyAnimeListId == request.MyAnimeListId)
            .Select(x => new MangaChaptersDto(
                x.CoverUrl,
                x.MyAnimeListId,
                x.AniListId,
                x.TitleRomaji,
                x.TitleEnglish,
                x.Chapters.Select(y => new ChaptersDto(y.SourceId, y.Number, y.Date, y.Url)).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}