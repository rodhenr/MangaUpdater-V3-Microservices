using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record EnsureMangaCommand(EnsureMangaRequest Manga) : IRequest<MangaDto>;

public class EnsureMangaHandler : IRequestHandler<EnsureMangaCommand, MangaDto>
{
    private readonly AppDbContext _context;

    public EnsureMangaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MangaDto> Handle(EnsureMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = await _context.Mangas
            .FirstOrDefaultAsync(
                x => x.AniListId == request.Manga.AniListId ||
                     (request.Manga.MyAnimeListId.HasValue && x.MyAnimeListId == request.Manga.MyAnimeListId),
                cancellationToken);

        if (manga is null)
        {
            manga = new Manga
            {
                AniListId = request.Manga.AniListId,
                MyAnimeListId = request.Manga.MyAnimeListId,
                TitleRomaji = request.Manga.TitleRomaji,
                TitleEnglish = request.Manga.TitleEnglish,
                CoverUrl = request.Manga.CoverUrl,
                IsAutoCreated = request.Manga.IsAutoCreated,
                LastUpdate = request.Manga.LastUpdate
            };

            _context.Mangas.Add(manga);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return new MangaDto(
            manga.Id,
            manga.MyAnimeListId,
            manga.AniListId,
            manga.TitleRomaji,
            manga.TitleEnglish,
            manga.CoverUrl,
            manga.IsAutoCreated,
            manga.LastUpdate,
            manga.Timestamp
        );
    }
}