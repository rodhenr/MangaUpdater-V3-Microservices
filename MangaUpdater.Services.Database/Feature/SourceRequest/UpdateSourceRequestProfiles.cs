using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceRequest;

public record UpdateSourceRequestProfileCommand(int SourceId, int ProfileId, UpdateSourceRequestProfileRequest Profile) : IRequest;

public class UpdateSourceRequestProfileHandler : IRequestHandler<UpdateSourceRequestProfileCommand>
{
    private readonly AppDbContext _context;

    public UpdateSourceRequestProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSourceRequestProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var profile = source.RequestProfiles.FirstOrDefault(candidate => candidate.Id == request.ProfileId)
                      ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "Request profile not found");

        SourceConfigurationSupport.ValidateVersion(request.Profile.Version);
        profile.IsActive = request.Profile.IsActive;
        profile.Version = request.Profile.Version;
        profile.Method = SourceConfigurationSupport.NormalizeRequiredValue(request.Profile.Method, "Method");
        profile.UrlTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.UrlTemplate);
        profile.HeadersJson = SourceConfigurationSupport.NormalizeValue(request.Profile.HeadersJson);
        profile.TimeoutSeconds = request.Profile.TimeoutSeconds;
        profile.UseCookies = request.Profile.UseCookies;
        profile.AcceptLanguage = SourceConfigurationSupport.NormalizeValue(request.Profile.AcceptLanguage);
        profile.Referrer = SourceConfigurationSupport.NormalizeValue(request.Profile.Referrer);
        profile.Timestamp = DateTime.UtcNow;

        SourceConfigurationSupport.ActivateRequestProfile(source, profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}