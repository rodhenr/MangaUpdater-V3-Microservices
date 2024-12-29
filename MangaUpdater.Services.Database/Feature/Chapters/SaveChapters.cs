using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.DTOs;

namespace MangaUpdater.Services.Database.Feature.Chapters;

public interface ISaveChapters
{
    Task SaveChaptersAsync(List<FetcherChapterResultDto> data, CancellationToken ct);
}

public class SaveChapters : ISaveChapters
{
    private readonly AppDbContext _context;

    public SaveChapters(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveChaptersAsync(List<FetcherChapterResultDto> data, CancellationToken ct)
    {
        var chapters = data
            .Select(x => new Chapter
            {
                MangaId = x.MangaId,
                SourceId = x.SourceId,
                Date = x.Date,
                Number = Convert.ToDecimal(x.Number),
            })
            .ToList();
        
        _context.Chapters.AddRange(chapters);
        await _context.SaveChangesAsync(ct);
    }
}