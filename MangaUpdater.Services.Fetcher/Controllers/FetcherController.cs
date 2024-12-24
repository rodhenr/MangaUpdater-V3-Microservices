using MangaUpdater.Services.Fetcher.Features.Factory;
using MangaUpdater.Services.Fetcher.Models;
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
    public async Task<List<ChapterResult>> GetChapters(ChapterRequest request, CancellationToken ct)
    {
        var service = _factory.GetChapterFetcher(request.Source);
        
        var dados = await service.GetChaptersAsync(request, ct);

        return dados;
    }
}