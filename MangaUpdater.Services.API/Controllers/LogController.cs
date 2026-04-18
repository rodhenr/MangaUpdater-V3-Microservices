using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc; 
using MangaUpdater.Services.API.Features.Logs;

namespace MangaUpdater.Services.API.Controllers;

public class LogController : BaseController
{
    private readonly ISender _sender;

    public LogController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("logs")]
    public async Task<PagedResultDto<LogEventDto>> GetLogs(int pageNumber = 1, int pageSize = 50)
    {
        return await _sender.Send(new GetLogsQuery(pageNumber, pageSize));
    }
}
