using MangaUpdater.Services.Database.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.MangaSources;

public record DeleteMangaSourceCommand(int MangaSourceId) : IRequest;

public class DeleteMangaSource : IRequestHandler<DeleteMangaSourceCommand>
{
    private readonly AppDbContext _context;
    
    public DeleteMangaSource(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(DeleteMangaSourceCommand request, CancellationToken cancellationToken)
    {
        var mangaSource = await _context.MangaSources
            .Where(x => x.Id == request.MangaSourceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (mangaSource == null) return;
        
        _context.MangaSources.Remove(mangaSource);
        await _context.SaveChangesAsync(cancellationToken);
    }
}