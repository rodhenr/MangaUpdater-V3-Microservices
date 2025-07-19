using MangaUpdater.Services.Database.Feature.Services;
using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class ServicesController : BaseController
{
    private readonly ISender _sender;

    public ServicesController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet("get-chapters/sources/{sourceId:int}")]
    public async Task<SourceDetails> GetStatus(int sourceId)
    {
        return await _sender.Send(new GetGetChaptersServiceStatusQuery((SourcesEnum)sourceId));
    }
    
    [HttpPost("get-chapters/sources/{sourceId:int}/pause")]
    public async Task Pause(int sourceId)
    {
        await _sender.Send(new PauseGetChaptersServiceCommand((SourcesEnum)sourceId));
    }

    [HttpPost("get-chapters/sources/{sourceId:int}/resume")]
    public async Task Resume(int sourceId)
    {
        await _sender.Send(new ResumeGetChaptersServiceCommand((SourcesEnum)sourceId));
    }

    [HttpPost("get-chapters/sources/{sourceId:int}/trigger")]
    public async Task TriggerNow(int sourceId)
    {
        await _sender.Send(new TriggerGetChapterServiceCommand((SourcesEnum)sourceId));
    }

    [HttpPost("get-chapters/sources/{sourceId:int}/delay")]
    public async Task SetDelay(int sourceId, [FromBody] ChapterServiceSetDelayRequest minutes)
    {
        await _sender.Send(new SetGetChaptersServiceDelayCommand((SourcesEnum)sourceId, minutes));
    }
}