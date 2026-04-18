using MangaUpdater.Services.AnilistConnector.Features.User;
using MangaUpdater.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.AnilistConnector.Controllers;

public class UserController : BaseController
{
    private readonly UserMangaCollection _userMangaCollection;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _userMangaCollection = new UserMangaCollection();
        _logger = logger;
    }

    [HttpGet("{username}")]
    public async Task<List<AnilistUserMangaCollectionDto>> GetUserMangaCollection(string username)
    {
        return await _userMangaCollection.GetCollection(username);
    }
}