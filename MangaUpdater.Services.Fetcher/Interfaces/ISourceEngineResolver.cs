using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Interfaces;

public interface ISourceEngineResolver
{
    IScraperEngine Resolve(SourceRuntimeDefinition sourceRuntimeDefinition);
}