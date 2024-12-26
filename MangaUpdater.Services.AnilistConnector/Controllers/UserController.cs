using MangaUpdater.Services.AnilistConnector.Features.User;
using MangaUpdater.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.AnilistConnector.Controllers;

public class UserController : BaseController
{
    private readonly UserMangaCollection _userMangaCollection;
    public UserController()
    {
        _userMangaCollection = new UserMangaCollection();
    }

    [HttpGet("{username}")]
    public async Task<List<AnilistUserMangaCollectionDto>> GetUserMangaCollection(string username)
    {
        return await _userMangaCollection.GetCollection(username);
    }
}