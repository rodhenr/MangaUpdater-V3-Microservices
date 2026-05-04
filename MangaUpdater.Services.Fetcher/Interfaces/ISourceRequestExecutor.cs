using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Interfaces;

public interface ISourceRequestExecutor
{
    Task<string> GetStringAsync(SourceRuntimeDefinition sourceRuntimeDefinition, string requestUrl,
        CancellationToken cancellationToken = default);

    void RegisterSuccess(SourceRuntimeDefinition sourceRuntimeDefinition);

    void RegisterConfigurationFailure(SourceRuntimeDefinition sourceRuntimeDefinition, string reason);
}