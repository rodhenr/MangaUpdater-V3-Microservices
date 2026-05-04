using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Helpers;
using MangaUpdater.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

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
        if (data.Count == 0)
            return;

        var mangaIds = data
            .Select(x => x.MangaId)
            .Distinct()
            .ToList();

        var sourceIds = data
            .Select(x => x.SourceId)
            .Distinct()
            .ToList();

        var existingChapters = await _context.Chapters
            .Where(chapter => mangaIds.Contains(chapter.MangaId) && sourceIds.Contains(chapter.SourceId))
            .ToListAsync(ct);

        var existingLookup = existingChapters.ToDictionary(
            chapter => BuildChapterKey(chapter.MangaId, chapter.SourceId, chapter.OriginalNumber),
            StringComparer.OrdinalIgnoreCase);

        foreach (var chapterDto in data)
        {
            var (major, minor, suffix) = ChapterNumberParser.Parse(chapterDto.Number);
            var normalizedDate = NormalizeUtc(chapterDto.Date);
            var key = BuildChapterKey(chapterDto.MangaId, chapterDto.SourceId, chapterDto.Number);

            if (existingLookup.TryGetValue(key, out var existingChapter))
            {
                existingChapter.Date = normalizedDate;
                existingChapter.Url = chapterDto.Url;
                existingChapter.NumberMajor = major;
                existingChapter.NumberMinor = minor;
                existingChapter.NumberSuffix = suffix;
                continue;
            }

            var chapter = new Chapter
            {
                MangaId = chapterDto.MangaId,
                SourceId = chapterDto.SourceId,
                Date = normalizedDate,
                OriginalNumber = chapterDto.Number,
                Url = chapterDto.Url,
                NumberMajor = major,
                NumberMinor = minor,
                NumberSuffix = suffix
            };

            _context.Chapters.Add(chapter);
            existingLookup[key] = chapter;
        }

        await _context.SaveChangesAsync(ct);
    }

    private static string BuildChapterKey(int mangaId, int sourceId, string originalNumber)
    {
        return $"{mangaId}:{sourceId}:{originalNumber.Trim()}";
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}