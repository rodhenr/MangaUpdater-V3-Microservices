using MangaUpdater.Services.Database.Feature.Mangas;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class MangaController : BaseController
{
    private readonly ISender _sender;
    public MangaController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("")]
    public async Task<List<MangaChaptersDto>> GetMangas()
    {
        return await _sender.Send(new GetMangasChaptersQuery());
    }
    
    [HttpGet("{malId:int}")]
    public async Task<List<MangaChaptersDto>> GetMangaById(int malId)
    {
        return await _sender.Send(new GetMangaChaptersQuery(malId));
    }
    
    [HttpPost("")]
    public async Task CreateManga([FromBody] CreateMangaRequest request)
    {
        await _sender.Send(new CreateMangaCommand(request));
    }
}