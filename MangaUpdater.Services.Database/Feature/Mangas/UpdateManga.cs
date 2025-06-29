using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record UpdateMangaCommand(int MangaId, UpdateMangaRequest Manga) : IRequest;

public class UpdateManga : IRequestHandler<UpdateMangaCommand>
{
    private readonly AppDbContext _context;
    public UpdateManga(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = await _context.Mangas
            .Where(x => x.Id == request.MangaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (manga is null) throw new HttpResponseException(HttpStatusCode.BadRequest, "Manga not found");
        
        manga.TitleRomaji = request.Manga.TitleRomaji;
        manga.TitleEnglish = request.Manga.TitleEnglish;
        manga.AniListId = request.Manga.AniListId;
        manga.MyAnimeListId = request.Manga.MyAnimeListId;
        manga.CoverUrl = request.Manga.CoverUrl;
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}