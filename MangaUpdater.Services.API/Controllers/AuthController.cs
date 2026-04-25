using MangaUpdater.Services.API.Features.Auth;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.API.Controllers;

public class AuthController : BaseController
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var dto = await _sender.Send(new LoginCommand(request));
        return Ok(dto);
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var dto = await _sender.Send(new GetMeQuery());
        return Ok(dto);
    }
}
