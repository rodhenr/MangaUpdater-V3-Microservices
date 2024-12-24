using MangaUpdater.Services.Database.Feature.Manga;
using MangaUpdater.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class MangaController : BaseController
{
    private readonly GetMangaChapters _getMangaChapters;
    public MangaController(GetMangaChapters getMangaChapters)
    {
        _getMangaChapters = getMangaChapters;
    }

    [HttpGet("{malId:int}")]
    public async Task<List<MangaChaptersDto>> GetMangas(int malId)
    {
        return await _getMangaChapters.GetMangaChaptersAsync(malId);
    }
}