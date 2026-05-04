using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Services.Fetcher.Services;
using MangaUpdater.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Fetcher.Controllers;

public class FetcherController : BaseController
{
    private readonly ScraperOrchestrator _scraperOrchestrator;

    public FetcherController(ScraperOrchestrator scraperOrchestrator)
    {
        _scraperOrchestrator = scraperOrchestrator;
    }
    
    [HttpPost]
    public async Task<List<ChapterResult>> UpdateChapters(ChapterQueueMessageDto request, CancellationToken ct)
    {
        return await _scraperOrchestrator.FetchAsync(request, ct);
    }
}