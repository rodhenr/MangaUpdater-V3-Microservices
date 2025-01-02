using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.MangaSources;

public record GetMangaSourcesQuery : IRequest<List<MangaSourceDto>>;

public class GetMangaSourcesHandler : IRequestHandler<GetMangaSourcesQuery, List<MangaSourceDto>>
{
    private readonly AppDbContext _context;

    public GetMangaSourcesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MangaSourceDto>> Handle(GetMangaSourcesQuery request, CancellationToken cancellationToken)
    {
        return await _context.MangaSources
            .Select(x => new MangaSourceDto(
                x.Id,
                x.MangaId,
                x.Manga.TitleEnglish,
                x.SourceId,
                x.Source.Name,
                x.Url
            ))
            .ToListAsync(cancellationToken);
    }
}