using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceApiProfiles;

public record CreateSourceApiProfileCommand(int SourceId, CreateSourceApiProfileRequest Profile) : IRequest;

public class CreateSourceApiProfileHandler : IRequestHandler<CreateSourceApiProfileCommand>
{
    private readonly AppDbContext _context;

    public CreateSourceApiProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateSourceApiProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        SourceConfigurationSupport.ValidateVersion(request.Profile.Version);

        var profile = new SourceApiProfile
        {
            SourceId = source.Id,
            IsActive = request.Profile.IsActive,
            Version = request.Profile.Version,
            HttpMethod = SourceConfigurationSupport.NormalizeRequiredValue(request.Profile.HttpMethod, "HttpMethod"),
            EndpointTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.EndpointTemplate),
            DataRootPath = SourceConfigurationSupport.NormalizeValue(request.Profile.DataRootPath),
            ChapterNumberPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberPath),
            ChapterDatePath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDatePath),
            ChapterUrlPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterUrlPath),
            ResultUrlTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.ResultUrlTemplate),
            PaginationMode = SourceConfigurationSupport.NormalizeValue(request.Profile.PaginationMode),
            OffsetParameterName = SourceConfigurationSupport.NormalizeValue(request.Profile.OffsetParameterName),
            LimitParameterName = SourceConfigurationSupport.NormalizeValue(request.Profile.LimitParameterName),
            NextPagePath = SourceConfigurationSupport.NormalizeValue(request.Profile.NextPagePath),
            ResultLimit = request.Profile.ResultLimit,
            Timestamp = DateTime.UtcNow
        };

        source.ApiProfiles.Add(profile);
        SourceConfigurationSupport.ActivateApiProfile(source, profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}