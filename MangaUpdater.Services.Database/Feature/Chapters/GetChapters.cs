using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Chapters;

public record GetChaptersQuery(int PageNumber, int PageSize) : IRequest<PagedResultDto<ChapterDto>>;

public class GetChaptersHandler : IRequestHandler<GetChaptersQuery, PagedResultDto<ChapterDto>>
{
    private readonly AppDbContext _context;

    public GetChaptersHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<ChapterDto>> Handle(GetChaptersQuery request, CancellationToken cancellationToken)
    {
        var mangasCount = await _context.Chapters.CountAsync(cancellationToken);
        
        var mangas = await _context.Chapters
            .OrderByDescending(x => x.Timestamp)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ChapterDto(
                x.Id,
                x.Manga.TitleEnglish,
                x.Source.Name,
                x.OriginalNumber,
                x.Timestamp
            ))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ChapterDto>(mangas, mangasCount, request.PageNumber,request.PageSize,(int)Math.Ceiling((double)mangasCount / request.PageSize));
    }
}