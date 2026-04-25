using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

[Authorize(Roles = "admin")]
public class AdminServicesController : BaseController
{
    private readonly ISender _sender;

    public AdminServicesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<List<Feature.AdminServices.AdminServiceStatusDto>> List()
    {
        return await _sender.Send(new Feature.AdminServices.GetAdminServicesQuery());
    }

    [HttpPost("{name}/start")]
    public async Task<ActionResult> Start(string name)
    {
        await _sender.Send(new Feature.AdminServices.StartAdminServiceCommand(name));
        return Ok();
    }

    [HttpPost("{name}/pause")]
    public async Task<ActionResult> Pause(string name)
    {
        await _sender.Send(new Feature.AdminServices.PauseAdminServiceCommand(name));
        return Ok();
    }

    [HttpPost("{name}/stop")]
    public async Task<ActionResult> Stop(string name)
    {
        var warning = await _sender.Send(new Feature.AdminServices.StopAdminServiceCommand(name));
        return Ok(new { warning });
    }

    private bool TryResolveSourceFromName(string name, out SourcesEnum source, out string error)
    {
        source = default;
        error = string.Empty;

        // Accept names like "ChapterDispatcher.Mangadex" or plain enum name "Mangadex"
        var candidate = name.Contains('.') ? name.Split('.').Last() : name;

        if (!Enum.TryParse<SourcesEnum>(candidate, ignoreCase: true, out source))
        {
            error = "Unknown service name";
            return false;
        }

        return true;
    }
}
