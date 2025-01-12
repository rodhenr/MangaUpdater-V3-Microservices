using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record CreateMangaCommand(CreateMangaRequest Manga) : IRequest;

public class CreateMangaHandler : IRequestHandler<CreateMangaCommand>
{
    private readonly AppDbContext _context;
    public CreateMangaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = new Manga
        {
            MyAnimeListId = request.Manga.MyAnimeListId,
            AniListId = request.Manga.AniListId,
            TitleEnglish = request.Manga.TitleEnglish,
            TitleRomaji = request.Manga.TitleRomaji,
            CoverUrl = request.Manga.CoverUrl,
        };

        _context.Mangas.Add(manga);
       await _context.SaveChangesAsync(cancellationToken);
    }
}