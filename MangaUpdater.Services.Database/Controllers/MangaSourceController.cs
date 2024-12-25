using MangaUpdater.Services.Database.Feature.MangaSources;
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

    [HttpPost("")]
    public async Task CreateMangaSource([FromBody] CreateMangaSourceRequest request)
    {
        await _sender.Send(new CreateMangaSourceCommand(request));
    }
}