using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Manga;

public class GetMangaChapters
{
    private readonly AppDbContext _context;

    public GetMangaChapters(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MangaChaptersDto>> GetMangaChaptersAsync(int myAnimeListId)
    {
        return await _context.Mangas
            .Where(x => x.MyAnimeListId == myAnimeListId)
            .Select(x => new MangaChaptersDto(
                x.MyAnimeListId,
                x.AniListId,
                x.TitleRomaji,
                x.TitleEnglish,
                x.Chapters.Select(y => new ChaptersDto(y.SourceId, y.Number, y.Date)).ToList()
            ))
            .ToListAsync();
    }
}