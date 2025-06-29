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
    
    [HttpGet]
    public async Task<PagedResultDto<MangaDto>> GetMangas(int pageNumber, int pageSize)
    {
        return await _sender.Send(new GetMangasQuery(pageNumber, pageSize));
    }

    [HttpGet("full")]
    public async Task<List<MangaChaptersDto>> GetMangasFull()
    {
        return await _sender.Send(new GetMangasChaptersQuery());
    }
    
    [HttpGet("{malId:int}")]
    public async Task<MangaChaptersDto?> GetMangaById(int malId)
    {
        return await _sender.Send(new GetMangaChaptersQuery(malId));
    }
    
    [HttpPost]
    public async Task CreateManga([FromBody] CreateMangaRequest request)
    {
        await _sender.Send(new CreateMangaCommand(request));
    }

    [HttpPut("{mangaId:int}")]
    public async Task UpdateManga(int mangaId, [FromBody] UpdateMangaRequest request)
    {
        await _sender.Send(new UpdateMangaCommand(mangaId, request));
    }
}