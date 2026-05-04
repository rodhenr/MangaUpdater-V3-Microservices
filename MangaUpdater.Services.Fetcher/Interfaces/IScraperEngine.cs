using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Interfaces;

public interface IScraperEngine
{
    string EngineType { get; }
    bool CanHandle(SourceRuntimeDefinition sourceRuntimeDefinition);
    Task<SourceParsingResult> FetchAsync(SourceRuntimeDefinition sourceRuntimeDefinition, GenericScraperContext context,
        CancellationToken cancellationToken = default);
}