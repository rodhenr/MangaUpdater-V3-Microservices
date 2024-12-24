using MangaUpdater.Services.Fetcher.Models;

namespace MangaUpdater.Services.Fetcher.Interfaces;

public interface IFetcher
{
    Task<List<ChapterResult>> GetChaptersAsync(ChapterRequest request, CancellationToken cancellationToken);
}