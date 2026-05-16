using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceRequest;

public record CreateSourceRequestProfileCommand(int SourceId, CreateSourceRequestProfileRequest Profile) : IRequest;

public class CreateSourceRequestProfileHandler : IRequestHandler<CreateSourceRequestProfileCommand>
{
    private readonly AppDbContext _context;

    public CreateSourceRequestProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateSourceRequestProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        SourceConfigurationSupport.ValidateVersion(request.Profile.Version);

        var profile = new SourceRequestProfile
        {
            SourceId = source.Id,
            IsActive = request.Profile.IsActive,
            Version = request.Profile.Version,
            Method = SourceConfigurationSupport.NormalizeRequiredValue(request.Profile.Method, "Method"),
            UrlTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.UrlTemplate),
            HeadersJson = SourceConfigurationSupport.NormalizeValue(request.Profile.HeadersJson),
            TimeoutSeconds = request.Profile.TimeoutSeconds,
            UseCookies = request.Profile.UseCookies,
            AcceptLanguage = SourceConfigurationSupport.NormalizeValue(request.Profile.AcceptLanguage),
            Referrer = SourceConfigurationSupport.NormalizeValue(request.Profile.Referrer),
            Timestamp = DateTime.UtcNow
        };

        source.RequestProfiles.Add(profile);
        SourceConfigurationSupport.ActivateRequestProfile(source, profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}