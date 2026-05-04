using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record GetSourceApiProfilesQuery(int SourceId) : IRequest<List<SourceApiProfileDto>>;
public record CreateSourceApiProfileCommand(int SourceId, CreateSourceApiProfileRequest Profile) : IRequest;
public record UpdateSourceApiProfileCommand(int SourceId, int ProfileId, UpdateSourceApiProfileRequest Profile) : IRequest;
public record DeleteSourceApiProfileCommand(int SourceId, int ProfileId) : IRequest;

public class GetSourceApiProfilesHandler : IRequestHandler<GetSourceApiProfilesQuery, List<SourceApiProfileDto>>
{
    private readonly AppDbContext _context;

    public GetSourceApiProfilesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SourceApiProfileDto>> Handle(GetSourceApiProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        return source.ApiProfiles
            .OrderByDescending(profile => profile.Version)
            .ThenByDescending(profile => profile.Id)
            .Select(profile => new SourceApiProfileDto(profile.Id, profile.SourceId, profile.IsActive, profile.Version,
                profile.HttpMethod, profile.EndpointTemplate, profile.DataRootPath, profile.ChapterNumberPath,
                profile.ChapterDatePath, profile.ChapterUrlPath, profile.ResultUrlTemplate, profile.PaginationMode,
                profile.OffsetParameterName, profile.LimitParameterName, profile.NextPagePath, profile.ResultLimit,
                profile.Timestamp))
            .ToList();
    }
}

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