using MangaUpdater.Services.Database.Feature.MangaSources;
using MangaUpdater.Services.Database.Feature.Sources;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class MangaSourceController : BaseController
{
    private readonly ISender _sender;
    public MangaSourceController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<List<MangaSourceDto>> GetMangaSources()
    {
        return await _sender.Send(new GetMangaSourcesQuery());
    }

    [HttpPost]
    public async Task CreateMangaSource([FromBody] CreateMangaSourceRequest request)
    {
        await _sender.Send(new CreateMangaSourceCommand(request));
    }
    
    
    [HttpGet("manga-distribution")]
    public async Task<List<GetMangaDistributionResponse>> GetMangaDistribution()
    {
        return await _sender.Send(new GetMangaDistributionQuery());
    }
    
}