using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Interfaces;

public interface IChapterExtractionService
{
    List<ChapterResult> ToChapterResults(GenericScraperContext context, SourceParsingResult parsingResult);
}