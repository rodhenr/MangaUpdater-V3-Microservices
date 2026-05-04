using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.MangaSources;

public record GetMangaSourcesToFetchQuery(int SourceId) : IRequest<List<ChapterQueueMessageDto>>;

public class GetMangaSourcesToFetchHandler : IRequestHandler<GetMangaSourcesToFetchQuery, List<ChapterQueueMessageDto>>
{
    private readonly AppDbContext _context;

    public GetMangaSourcesToFetchHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChapterQueueMessageDto>> Handle(GetMangaSourcesToFetchQuery request, CancellationToken cancellationToken)
    {
        return await _context.MangaSources
            .Where(x => x.SourceId == request.SourceId)
            .Select(x => new ChapterQueueMessageDto(
                x.SourceId,
                x.Source.Slug,
                x.Source.BaseUrl,
                x.Url,
                x.AdditionalInfo,
                x.MangaId,
                x.Manga.TitleEnglish,
                x.Manga.Chapters.Any() 
                    ? x.Manga.Chapters.OrderByDescending(c => c.NumberMajor).ThenByDescending(c => c.NumberMinor).ThenByDescending(c => c.NumberSuffix).First().OriginalNumber 
                    : "0"
            ))
            .ToListAsync(cancellationToken);
    }
}