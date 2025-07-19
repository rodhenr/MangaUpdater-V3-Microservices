using MangaUpdater.Services.Database.Feature.Services;
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
    
    [HttpGet("get-chapters")]
    public async Task<GetChaptersServiceStatusDto> GetStatus()
    {
        return await _sender.Send(new GetGetChaptersServiceStatusQuery());
    }
    
    [HttpPost("get-chapters/pause")]
    public async Task Pause()
    {
        await _sender.Send(new PauseGetChaptersServiceCommand());
    }

    [HttpPost("get-chapters/resume")]
    public async Task Resume()
    {
        await _sender.Send(new ResumeGetChaptersServiceCommand());
    }

    [HttpPost("get-chapters/trigger")]
    public async Task TriggerNow()
    {
        await _sender.Send(new TriggerGetChapterServiceCommand());
    }

    [HttpPost("get-chapters/delay")]
    public async Task SetDelay([FromBody] ChapterServiceSetDelayRequest minutes)
    {
        await _sender.Send(new SetGetChaptersServiceDelayCommand(minutes));
    }
}