using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record UpdateSourceCommand(int SourceId, UpdateSourceRequest Source) : IRequest;

public class UpdateSourceHandler : IRequestHandler<UpdateSourceCommand>
{
    private readonly AppDbContext _context;
    public UpdateSourceHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSourceCommand request, CancellationToken cancellationToken)
    {
        var source = await _context.Sources
            .Where(x => x.Id == request.SourceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (source is null) throw new HttpResponseException(HttpStatusCode.BadRequest, "Source not found");

        source.Name = request.Source.Name;
        source.BaseUrl = request.Source.Url;
        source.Timestamp = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
