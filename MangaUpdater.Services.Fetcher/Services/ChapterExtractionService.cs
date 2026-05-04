using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Services;

public class ChapterExtractionService : IChapterExtractionService
{
    public List<ChapterResult> ToChapterResults(GenericScraperContext context, SourceParsingResult parsingResult)
    {
        return parsingResult.Chapters
            .Select(chapter => new ChapterResult(
                context.MangaId,
                context.MangaName,
                parsingResult.SourceId,
                chapter.Number,
                chapter.Date,
                chapter.Url))
            .ToList();
    }
}