using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Logs;

public record GetLogsQuery : IRequest<List<LogEvent>>;

public class GetLogsHandler : IRequestHandler<GetLogsQuery, List<LogEvent>>
{
    private readonly AppDbContext _context;
    
    public GetLogsHandler(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<LogEvent>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement pagination
        return await _context.LogEvents.ToListAsync(cancellationToken);
    }
}