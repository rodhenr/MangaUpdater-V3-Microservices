using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Services;

public class ScraperOrchestrator
{
    private readonly ISourceDefinitionProvider _sourceDefinitionProvider;
    private readonly ISourceEngineResolver _sourceEngineResolver;
    private readonly IChapterExtractionService _chapterExtractionService;

    public ScraperOrchestrator(ISourceDefinitionProvider sourceDefinitionProvider,
        ISourceEngineResolver sourceEngineResolver, IChapterExtractionService chapterExtractionService)
    {
        _sourceDefinitionProvider = sourceDefinitionProvider;
        _sourceEngineResolver = sourceEngineResolver;
        _chapterExtractionService = chapterExtractionService;
    }

    public async Task<List<ChapterResult>> FetchAsync(ChapterQueueMessageDto request,
        CancellationToken cancellationToken = default)
    {
        var sourceRuntimeDefinition = await _sourceDefinitionProvider.GetSourceDefinitionAsync(request, cancellationToken);
        var context = new GenericScraperContext(
            sourceRuntimeDefinition.SourceId,
            sourceRuntimeDefinition.SourceSlug,
            sourceRuntimeDefinition.SourceBaseUrl,
            request.MangaPath,
            request.AdditionalInfo,
            request.MangaId,
            request.MangaName,
            request.LastChapterNumber);
        
        var engine = _sourceEngineResolver.Resolve(sourceRuntimeDefinition);
        var parsingResult = await engine.FetchAsync(sourceRuntimeDefinition, context, cancellationToken);

        return _chapterExtractionService.ToChapterResults(context, parsingResult);
    }
}