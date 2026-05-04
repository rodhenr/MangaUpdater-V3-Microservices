using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Services;

public class SourceEngineResolver : ISourceEngineResolver
{
    private readonly IReadOnlyCollection<IScraperEngine> _scraperEngines;

    public SourceEngineResolver(IEnumerable<IScraperEngine> scraperEngines)
    {
        _scraperEngines = scraperEngines.ToList();
    }

    public IScraperEngine Resolve(SourceRuntimeDefinition sourceRuntimeDefinition)
    {
        var engine = _scraperEngines.FirstOrDefault(candidate => candidate.CanHandle(sourceRuntimeDefinition));
        if (engine is not null)
            return engine;

        var customEngine = _scraperEngines.FirstOrDefault(candidate =>
            string.Equals(candidate.EngineType, "Custom", StringComparison.OrdinalIgnoreCase));

        return customEngine ?? throw new InvalidOperationException(
            $"No scraper engine registered for engine type '{sourceRuntimeDefinition.EngineType}'.");
    }
}