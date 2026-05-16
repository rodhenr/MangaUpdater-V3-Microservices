using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Exceptions;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceScrapingProfiles;

public record DeleteSourceScrapingProfileCommand(int SourceId, int ProfileId) : IRequest;

public class DeleteSourceScrapingProfileHandler : IRequestHandler<DeleteSourceScrapingProfileCommand>
{
    private readonly AppDbContext _context;

    public DeleteSourceScrapingProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSourceScrapingProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var profile = source.ScrapingProfiles.FirstOrDefault(candidate => candidate.Id == request.ProfileId)
                      ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "Scraping profile not found");

        _context.SourceScrapingProfiles.Remove(profile);
        source.ScrapingProfiles.Remove(profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}