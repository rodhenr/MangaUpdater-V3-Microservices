using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
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
        var maxNumbers = await _context.Chapters
            .GroupBy(c => new { c.MangaId, c.SourceId })
            .Select(g => new 
            {
                g.Key.MangaId,
                g.Key.SourceId,
                MaxNumber = g.Max(c => c.Number)
            })
            .ToListAsync(ct);
        
        var newChapters = data
            .Where(d => !maxNumbers.Any(mn => 
                mn.MangaId == d.MangaId &&
                mn.SourceId == d.SourceId &&
                mn.MaxNumber >= Convert.ToDecimal(d.Number)))
            .Select(x => new Chapter
            {
                MangaId = x.MangaId,
                SourceId = x.SourceId,
                Date = x.Date,
                Number = Convert.ToDecimal(x.Number),
            })
            .ToList();
        
        var uniqueChapters = newChapters
            .GroupBy(c => new { c.MangaId, c.SourceId, c.Number })
            .Select(g => g.First())
            .ToList();
        
        _context.Chapters.AddRange(uniqueChapters);
        await _context.SaveChangesAsync(ct);
    }
}