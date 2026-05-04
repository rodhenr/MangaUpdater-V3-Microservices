using MangaUpdater.Services.Database.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record ActiveDispatchSourceDto(int SourceId, string SourceName, string QueueName);

public record GetActiveDispatchSourcesQuery() : IRequest<List<ActiveDispatchSourceDto>>;

public class GetActiveDispatchSourcesHandler : IRequestHandler<GetActiveDispatchSourcesQuery, List<ActiveDispatchSourceDto>>
{
    private readonly AppDbContext _context;

    public GetActiveDispatchSourcesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActiveDispatchSourceDto>> Handle(GetActiveDispatchSourcesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Sources
            .AsNoTracking()
            .Where(source => source.IsEnabled)
            .OrderBy(source => source.Id)
            .Select(source => new ActiveDispatchSourceDto(
                source.Id,
                source.Name,
                string.IsNullOrWhiteSpace(source.QueueName)
                    ? $"get-chapters-{source.Slug ?? source.Id.ToString()}"
                    : source.QueueName))
            .ToListAsync(cancellationToken);
    }
}