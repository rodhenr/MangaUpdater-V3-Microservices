using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Helpers;
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
            .Select(x =>
            {
                var (major, minor, suffix) = ChapterNumberParser.Parse(x.Number);

                return new Chapter
                {
                    MangaId = x.MangaId,
                    SourceId = x.SourceId,
                    Date = x.Date,
                    OriginalNumber = x.Number,
                    Url = x.Url,
                    NumberMajor = major,
                    NumberMinor = minor,
                    NumberSuffix = suffix
                };
            })
            .ToList();
        
        _context.Chapters.AddRange(chapters);
        await _context.SaveChangesAsync(ct);
    }
}