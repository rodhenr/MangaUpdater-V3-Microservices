using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;

namespace MangaUpdater.Services.Fetcher.Interfaces;

public interface IFetcher
{
    Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken = default);
}