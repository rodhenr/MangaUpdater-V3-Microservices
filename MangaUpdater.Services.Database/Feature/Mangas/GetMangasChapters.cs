using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record GetMangasChaptersQuery : IRequest<List<MangaChaptersDto>>;

public class GetMangasChaptersHandler : IRequestHandler<GetMangasChaptersQuery, List<MangaChaptersDto>>
{
    private readonly AppDbContext _context;

    public GetMangasChaptersHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MangaChaptersDto>> Handle(GetMangasChaptersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Mangas
            .Select(x => new MangaChaptersDto(
                "",
                x.MyAnimeListId,
                x.AniListId,
                x.TitleRomaji,
                x.TitleEnglish,
                x.Chapters.Select(y => new ChaptersDto(y.SourceId, y.OriginalNumber, y.Date, y.Url)).ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}