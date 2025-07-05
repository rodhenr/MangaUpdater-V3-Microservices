using MangaUpdater.Services.Database.Feature.Sources;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class SourceController : BaseController
{
    private readonly ISender _sender;

    public SourceController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<PagedResultDto<SourceDto>> GetSources(int pageNumber, int pageSize)
    {
        return await _sender.Send(new GetSourcesQuery(pageNumber, pageSize));
    }
    
    [HttpPost]
    public async Task CreateSource([FromBody] CreateSourceRequest request)
    {
        await _sender.Send(new CreateSourceCommand(request));
    }
}