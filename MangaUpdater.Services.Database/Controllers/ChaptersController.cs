using MangaUpdater.Services.Database.Feature.Chapters;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class ChaptersController : BaseController
{
    private readonly ISender _sender;

    public ChaptersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<PagedResultDto<ChapterDto>> GetChapters(int pageNumber, int pageSize)
    {
        return await _sender.Send(new GetChaptersQuery(pageNumber, pageSize));
    }
}