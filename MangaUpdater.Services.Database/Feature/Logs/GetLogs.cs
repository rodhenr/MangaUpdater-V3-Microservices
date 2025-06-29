using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Logs;

public record GetLogsQuery(int PageNumber, int PageSize) : IRequest<PagedResultDto<LogEvent>>;

public class GetLogsHandler : IRequestHandler<GetLogsQuery, PagedResultDto<LogEvent>>
{
    private readonly AppDbContext _context;
    
    public GetLogsHandler(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<PagedResultDto<LogEvent>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var logsCount = await _context.LogEvents.CountAsync(cancellationToken);
        
        var logs = await _context.LogEvents
            .OrderByDescending(x => x.Timestamp)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<LogEvent>(logs, logsCount, request.PageNumber, request.PageSize, (int)Math.Ceiling((double)logsCount / request.PageSize));
    }
}