using MangaUpdater.Services.Database.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Metrics;

public record GetMetricsQuery : IRequest<GetMetricsResponse>;
public record GetMetricsResponse(int MangasCount, int SourcesCount, int RelationsCount, int LogsLastDayCount, int QueueCount);

public class GetMetrics: IRequestHandler<GetMetricsQuery, GetMetricsResponse>
{
    private readonly AppDbContext _context;
    
    public GetMetrics(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetMetricsResponse> Handle(GetMetricsQuery request, CancellationToken cancellationToken)
    {
        var mangasCount = await _context.Mangas.CountAsync(cancellationToken: cancellationToken);
        var sourcesCount = await _context.Sources.CountAsync(cancellationToken: cancellationToken);
        var mangaSourcesCount = await _context.MangaSources.CountAsync(cancellationToken: cancellationToken);
        var logsCount = await _context.LogEvents.Where(x => x.Timestamp >= DateTime.UtcNow.AddHours(-24)).CountAsync(cancellationToken: cancellationToken);
        
        return new GetMetricsResponse(mangasCount, sourcesCount,mangaSourcesCount, logsCount, 0);
    }
}