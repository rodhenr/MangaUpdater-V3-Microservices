using MangaUpdater.Services.AnilistConnector.Features.User;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.AnilistConnector.Controllers;

public class UserController : BaseController
{
    private readonly ISender _sender;
    private readonly ILogger<UserController> _logger;

    public UserController(ISender sender, ILogger<UserController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpGet("{username}")]
    public async Task<List<AnilistUserMangaCollectionDto>> GetUserMangaCollection(string username)
    {
        return await _sender.Send(new GetUserMangaCollectionQuery(username));
    }
}