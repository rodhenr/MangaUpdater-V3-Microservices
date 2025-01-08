using MangaUpdater.Services.Database.Database;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Logs;

public record GetLogsQuery() : IRequest<List<GetLogsResponse>>;

public record GetLogsResponse();

public class GetLogsHandler : IRequestHandler<GetLogsQuery, List<GetLogsResponse>>
{
    private readonly AppDbContext _context;
    
    public GetLogsHandler(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<GetLogsResponse>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}