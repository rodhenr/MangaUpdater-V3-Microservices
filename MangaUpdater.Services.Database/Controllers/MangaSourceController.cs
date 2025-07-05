using MangaUpdater.Services.Database.Feature.MangaSources;
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
    public async Task<PagedResultDto<MangaSourceDto>> GetMangaSources(int pageNumber, int pageSize)
    {
        return await _sender.Send(new GetMangaSourcesQuery(pageNumber, pageSize));
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
    
    [HttpPut("{mangaSourceId:int}")]
    public async Task UpdateManga(int mangaSourceId, [FromBody] UpdateMangaSourceRequest request)
    {
        await _sender.Send(new UpdateMangaSourceCommand(mangaSourceId, request));
    }
    
    [HttpDelete("{mangaSourceId:int}")]
    public async Task DeleteManga(int mangaSourceId)
    {
        await _sender.Send(new DeleteMangaSourceCommand(mangaSourceId));
    }
}