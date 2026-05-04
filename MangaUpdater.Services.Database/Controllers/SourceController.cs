using MangaUpdater.Services.Database.Feature.Sources;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

[Authorize(Roles = "admin")]
public class SourceController : BaseController
{
    private readonly ISender _sender;

    public SourceController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<PagedResultDto<SourceDto>> GetSources(int pageNumber, int pageSize)
    {
        return await _sender.Send(new GetSourcesQuery(pageNumber, pageSize));
    }
    
    [HttpPost]
    public async Task CreateSource([FromBody] CreateSourceRequest request)
    {
        await _sender.Send(new CreateSourceCommand(request));
    }

    [HttpPut("{sourceId:int}")]
    public async Task UpdateSource(int sourceId, [FromBody] UpdateSourceRequest request)
    {
        await _sender.Send(new UpdateSourceCommand(sourceId, request));
    }

    [HttpGet("{sourceId:int}/request-profiles")]
    public async Task<List<SourceRequestProfileDto>> GetRequestProfiles(int sourceId)
    {
        return await _sender.Send(new GetSourceRequestProfilesQuery(sourceId));
    }

    [HttpPost("{sourceId:int}/request-profiles")]
    public async Task CreateRequestProfile(int sourceId, [FromBody] CreateSourceRequestProfileRequest request)
    {
        await _sender.Send(new CreateSourceRequestProfileCommand(sourceId, request));
    }

    [HttpPut("{sourceId:int}/request-profiles/{profileId:int}")]
    public async Task UpdateRequestProfile(int sourceId, int profileId, [FromBody] UpdateSourceRequestProfileRequest request)
    {
        await _sender.Send(new UpdateSourceRequestProfileCommand(sourceId, profileId, request));
    }

    [HttpDelete("{sourceId:int}/request-profiles/{profileId:int}")]
    public async Task DeleteRequestProfile(int sourceId, int profileId)
    {
        await _sender.Send(new DeleteSourceRequestProfileCommand(sourceId, profileId));
    }

    [HttpGet("{sourceId:int}/api-profiles")]
    public async Task<List<SourceApiProfileDto>> GetApiProfiles(int sourceId)
    {
        return await _sender.Send(new GetSourceApiProfilesQuery(sourceId));
    }

    [HttpPost("{sourceId:int}/api-profiles")]
    public async Task CreateApiProfile(int sourceId, [FromBody] CreateSourceApiProfileRequest request)
    {
        await _sender.Send(new CreateSourceApiProfileCommand(sourceId, request));
    }

    [HttpPut("{sourceId:int}/api-profiles/{profileId:int}")]
    public async Task UpdateApiProfile(int sourceId, int profileId, [FromBody] UpdateSourceApiProfileRequest request)
    {
        await _sender.Send(new UpdateSourceApiProfileCommand(sourceId, profileId, request));
    }

    [HttpDelete("{sourceId:int}/api-profiles/{profileId:int}")]
    public async Task DeleteApiProfile(int sourceId, int profileId)
    {
        await _sender.Send(new DeleteSourceApiProfileCommand(sourceId, profileId));
    }

    [HttpGet("{sourceId:int}/scraping-profiles")]
    public async Task<List<SourceScrapingProfileDto>> GetScrapingProfiles(int sourceId)
    {
        return await _sender.Send(new GetSourceScrapingProfilesQuery(sourceId));
    }

    [HttpPost("{sourceId:int}/scraping-profiles")]
    public async Task CreateScrapingProfile(int sourceId, [FromBody] CreateSourceScrapingProfileRequest request)
    {
        await _sender.Send(new CreateSourceScrapingProfileCommand(sourceId, request));
    }

    [HttpPut("{sourceId:int}/scraping-profiles/{profileId:int}")]
    public async Task UpdateScrapingProfile(int sourceId, int profileId,
        [FromBody] UpdateSourceScrapingProfileRequest request)
    {
        await _sender.Send(new UpdateSourceScrapingProfileCommand(sourceId, profileId, request));
    }

    [HttpDelete("{sourceId:int}/scraping-profiles/{profileId:int}")]
    public async Task DeleteScrapingProfile(int sourceId, int profileId)
    {
        await _sender.Send(new DeleteSourceScrapingProfileCommand(sourceId, profileId));
    }

    [HttpPost("{sourceId:int}/validate-profile")]
    public async Task<SourceProfileValidationResultDto> ValidateProfile(int sourceId,
        [FromBody] ValidateSourceProfileRequest request)
    {
        return await _sender.Send(new ValidateSourceProfileQuery(sourceId, request));
    }
}