using MangaUpdater.Services.Fetcher.Services;
using MangaUpdater.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Fetcher.Controllers;

public class FetcherController : BaseController
{
    private readonly FetcherFactory _factory;
    
    public FetcherController(FetcherFactory factory)
    {
        _factory = factory;
    }
    
    [HttpPost]
    public async Task UpdateChapters(ChapterQueueMessageDto request, CancellationToken ct)
    {
        var service = _factory.GetChapterFetcher(request.Source);
        var data = await service.GetChaptersAsync(request, ct);
    }
}