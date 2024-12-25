using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.MangaSources;

public record CreateMangaSourceCommand(CreateMangaSourceRequest Data) : IRequest;

public class CreateMangaSourceHandler : IRequestHandler<CreateMangaSourceCommand>
{
    private readonly AppDbContext _context;

    public CreateMangaSourceHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateMangaSourceCommand request, CancellationToken cancellationToken)
    {
        var mangaSource = new MangaSource
        {
            MangaId = request.Data.MangaId,
            SourceId = request.Data.SourceId,
            Url = request.Data.Url,
        };

        _context.MangaSources.Add(mangaSource);
        await _context.SaveChangesAsync(cancellationToken);
    }
}