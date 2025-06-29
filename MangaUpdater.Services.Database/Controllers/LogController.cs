using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Feature.Logs;
using MangaUpdater.Shared.DTOs;
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
    public async Task<PagedResultDto<LogEvent>> GetLogs(int pageNumber, int pageSize)
    {
        return await _sender.Send(new GetLogsQuery(pageNumber, pageSize));
    }
}