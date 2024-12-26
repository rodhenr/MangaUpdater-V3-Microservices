using MangaUpdater.Services.API.DTOs;
using MangaUpdater.Services.API.Features.Info;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.API.Controllers;

public class InfoController : BaseController
{
    private readonly ISender _sender;

    public InfoController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("user/{username}/chapters")]
    public async Task<List<UserChaptersDto>> GetUserChaptersInfo(string username)
    {
        return await _sender.Send(new UserChaptersQuery(username));
    }
}