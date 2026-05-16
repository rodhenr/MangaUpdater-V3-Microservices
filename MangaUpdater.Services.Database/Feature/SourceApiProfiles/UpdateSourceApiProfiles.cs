using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.SourceConfigurations;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.SourceApiProfiles;

public record UpdateSourceApiProfileCommand(int SourceId, int ProfileId, UpdateSourceApiProfileRequest Profile) : IRequest;

public class UpdateSourceApiProfileHandler : IRequestHandler<UpdateSourceApiProfileCommand>
{
    private readonly AppDbContext _context;

    public UpdateSourceApiProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSourceApiProfileCommand request, CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var profile = source.ApiProfiles.FirstOrDefault(candidate => candidate.Id == request.ProfileId)
                      ?? throw new HttpResponseException(HttpStatusCode.BadRequest, "API profile not found");

        SourceConfigurationSupport.ValidateVersion(request.Profile.Version);
        profile.IsActive = request.Profile.IsActive;
        profile.Version = request.Profile.Version;
        profile.HttpMethod = SourceConfigurationSupport.NormalizeRequiredValue(request.Profile.HttpMethod, "HttpMethod");
        profile.EndpointTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.EndpointTemplate);
        profile.DataRootPath = SourceConfigurationSupport.NormalizeValue(request.Profile.DataRootPath);
        profile.ChapterNumberPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterNumberPath);
        profile.ChapterDatePath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterDatePath);
        profile.ChapterUrlPath = SourceConfigurationSupport.NormalizeValue(request.Profile.ChapterUrlPath);
        profile.ResultUrlTemplate = SourceConfigurationSupport.NormalizeValue(request.Profile.ResultUrlTemplate);
        profile.PaginationMode = SourceConfigurationSupport.NormalizeValue(request.Profile.PaginationMode);
        profile.OffsetParameterName = SourceConfigurationSupport.NormalizeValue(request.Profile.OffsetParameterName);
        profile.LimitParameterName = SourceConfigurationSupport.NormalizeValue(request.Profile.LimitParameterName);
        profile.NextPagePath = SourceConfigurationSupport.NormalizeValue(request.Profile.NextPagePath);
        profile.ResultLimit = request.Profile.ResultLimit;
        profile.Timestamp = DateTime.UtcNow;

        SourceConfigurationSupport.ActivateApiProfile(source, profile);
        SourceConfigurationSupport.ValidateSourceConfigurationOrThrow(source);
        await _context.SaveChangesAsync(cancellationToken);
    }
}