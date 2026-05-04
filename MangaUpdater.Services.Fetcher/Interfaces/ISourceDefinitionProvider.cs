using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Interfaces;

public interface ISourceDefinitionProvider
{
    Task<SourceRuntimeDefinition> GetSourceDefinitionAsync(ChapterQueueMessageDto request,
        CancellationToken cancellationToken = default);
}