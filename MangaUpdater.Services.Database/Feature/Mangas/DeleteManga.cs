using MangaUpdater.Services.Database.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Mangas;

public record DeleteMangaCommand(int MangaId) : IRequest;

public class DeleteManga : IRequestHandler<DeleteMangaCommand>
{
    private readonly AppDbContext _context;
    
    public DeleteManga(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(DeleteMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = await _context.Mangas
            .Where(x => x.Id == request.MangaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (manga == null) return;
        
        _context.Mangas.Remove(manga);
        await _context.SaveChangesAsync(cancellationToken);
    }
}