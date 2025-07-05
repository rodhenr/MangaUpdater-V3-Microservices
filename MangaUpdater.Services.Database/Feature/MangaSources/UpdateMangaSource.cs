using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.MangaSources;

public record UpdateMangaSourceCommand(int MangaSourceId, UpdateMangaSourceRequest MangaSource) : IRequest;


public class UpdateMangaSource : IRequestHandler<UpdateMangaSourceCommand>
{
    private readonly AppDbContext _context;
    public UpdateMangaSource(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(UpdateMangaSourceCommand request, CancellationToken cancellationToken)
    {
        var mangaSource = await _context.MangaSources
            .Where(x => x.Id == request.MangaSourceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (mangaSource is null) throw new HttpResponseException(HttpStatusCode.BadRequest, "Manga Source not found");
        
        mangaSource.MangaId = request.MangaSource.MangaId;
        mangaSource.SourceId = request.MangaSource.SourceId;
        mangaSource.Timestamp = DateTime.UtcNow;
        mangaSource.Url = request.MangaSource.Url;
        mangaSource.AdditionalInfo = request.MangaSource.AdditionalInfo;
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}