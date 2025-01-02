using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record CreateSourceCommand(CreateSourceRequest Source) : IRequest;

public class CreateSourceHandler : IRequestHandler<CreateSourceCommand>
{
    private readonly AppDbContext _context;
    public CreateSourceHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateSourceCommand request, CancellationToken cancellationToken)
    {
        var source = new Source
        {
            Name = request.Source.Name,
            BaseUrl = request.Source.Url
        };

        _context.Sources.Add(source);
       await _context.SaveChangesAsync(cancellationToken);
    }
}