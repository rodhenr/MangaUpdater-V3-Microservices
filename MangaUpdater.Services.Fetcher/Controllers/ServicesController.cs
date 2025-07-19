using MangaUpdater.Services.Fetcher.Features.Services;
using MangaUpdater.Shared.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Fetcher.Controllers;

public class ServicesController : BaseController
{
    private readonly ISender _sender;

    public ServicesController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet("queue")]
    public async Task<Dictionary<string, ServicesStateEnum>> GetQueueStatus()
    {
        return await _sender.Send(new GetQueueStatesQuery());
    }
    
    [HttpPost("queue/pause")]
    public async Task PauseQueue([FromBody] string queueName)
    {
        await _sender.Send(new PauseQueueCommand(queueName));
    }

    [HttpPost("queue/resume")]
    public async Task ResumeQueue([FromBody] string queueName)
    {
        await _sender.Send(new ResumeQueueCommand(queueName));
    }
}