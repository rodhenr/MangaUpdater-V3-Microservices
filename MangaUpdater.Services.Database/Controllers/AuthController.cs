using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class AuthController : BaseController
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequest request)
    {
        var response = await _sender.Send(new Feature.Auth.LoginCommand(request));
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<LoginUserDto>> Me()
    {
        var response = await _sender.Send(new Feature.Auth.GetMeQuery());
        return Ok(response);
    }
}
