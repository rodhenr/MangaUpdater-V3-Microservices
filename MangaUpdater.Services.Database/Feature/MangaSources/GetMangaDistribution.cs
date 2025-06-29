using MangaUpdater.Services.Database.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.MangaSources;

public record GetMangaDistributionQuery : IRequest<List<GetMangaDistributionResponse>>;

public record GetMangaDistributionResponse(int MangaCount, string SourceName);

public class GetMangaDistribution : IRequestHandler<GetMangaDistributionQuery, List<GetMangaDistributionResponse>>
{
    private readonly AppDbContext _context;
    public GetMangaDistribution(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<GetMangaDistributionResponse>> Handle(GetMangaDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _context.MangaSources
            .GroupBy(x => new { x.SourceId, x.Source.Name })
            .Select(x => new GetMangaDistributionResponse(x.Count(), x.Key.Name))
            .ToListAsync(cancellationToken);
    }
}