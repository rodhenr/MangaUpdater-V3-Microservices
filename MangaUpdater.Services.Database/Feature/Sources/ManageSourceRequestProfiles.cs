using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record GetSourceRequestProfilesQuery(int SourceId) : IRequest<List<SourceRequestProfileDto>>;
public record CreateSourceRequestProfileCommand(int SourceId, CreateSourceRequestProfileRequest Profile) : IRequest;
public record UpdateSourceRequestProfileCommand(int SourceId, int ProfileId, UpdateSourceRequestProfileRequest Profile) : IRequest;
public record DeleteSourceRequestProfileCommand(int SourceId, int ProfileId) : IRequest;

public class GetSourceRequestProfilesHandler : IRequestHandler<GetSourceRequestProfilesQuery, List<SourceRequestProfileDto>>
{
    private readonly AppDbContext _context;

    public GetSourceRequestProfilesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SourceRequestProfileDto>> Handle(GetSourceRequestProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        return source.RequestProfiles
            .OrderByDescending(profile => profile.Version)
            .ThenByDescending(profile => profile.Id)
            .Select(Map)
            .ToList();
    }

    private static SourceRequestProfileDto Map(SourceRequestProfile profile)
    {
        return new SourceRequestProfileDto(profile.Id, profile.SourceId, profile.IsActive, profile.Version, profile.Method,
            profile.UrlTemplate, profile.HeadersJson, profile.TimeoutSeconds, profile.UseCookies, profile.AcceptLanguage,
            profile.Referrer, profile.Timestamp);
    }
}

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