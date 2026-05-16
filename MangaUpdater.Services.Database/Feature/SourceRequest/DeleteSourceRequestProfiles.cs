using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Exceptions;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceRequest;

public record DeleteSourceRequestProfileCommand(int SourceId, int ProfileId) : IRequest;

public class DeleteSourceRequestProfileHandler : IRequestHandler<DeleteSourceRequestProfileCommand>
{
    private readonly AppDbContext _context;

    public DeleteSourceRequestProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSourceRequestProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var profile = source.RequestProfiles.FirstOrDefault(candidate => candidate.Id == request.ProfileId)
                      ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "Request profile not found");

        _context.SourceRequestProfiles.Remove(profile);
        source.RequestProfiles.Remove(profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}