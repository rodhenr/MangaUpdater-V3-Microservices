using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Exceptions;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceApiProfiles;

public record DeleteSourceApiProfileCommand(int SourceId, int ProfileId) : IRequest;

public class DeleteSourceApiProfileHandler : IRequestHandler<DeleteSourceApiProfileCommand>
{
    private readonly AppDbContext _context;

    public DeleteSourceApiProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSourceApiProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var profile = source.ApiProfiles.FirstOrDefault(candidate => candidate.Id == request.ProfileId)
                      ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "API profile not found");

        _context.SourceApiProfiles.Remove(profile);
        source.ApiProfiles.Remove(profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}