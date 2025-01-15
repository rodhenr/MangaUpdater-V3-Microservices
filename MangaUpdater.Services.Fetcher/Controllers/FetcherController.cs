using MangaUpdater.Services.Fetcher.Models;
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
    public async Task<List<ChapterResult>> UpdateChapters(ChapterQueueMessageDto request, CancellationToken ct)
    {
        var service = _factory.GetChapterFetcher(request.Source);
        return await service.GetChaptersAsync(request, ct);
    }
}