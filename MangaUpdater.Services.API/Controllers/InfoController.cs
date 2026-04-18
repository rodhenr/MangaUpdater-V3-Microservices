using MangaUpdater.Services.API.DTOs;
using MangaUpdater.Services.API.Features.Info;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.API.Controllers;

public class InfoController : BaseController
{
    private readonly ISender _sender;
    private readonly ILogger<InfoController> _logger;

    public InfoController(ISender sender, ILogger<InfoController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpGet("user/{username}/chapters")]
    public async Task<List<UserChaptersDto>> GetUserChaptersInfo(string username)
    {
        return await _sender.Send(new UserChaptersQuery(username));
    }
}