using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Feature.Logs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class LogController : BaseController
{
    private readonly ISender _sender;

    public LogController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<List<LogEvent>> GetLogs()
    {
        return await _sender.Send(new GetLogsQuery());
    }
}