using System.Security.Claims;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MangaUpdater.Services.Database.Feature.Auth;

public record GetMeQuery() : IRequest<LoginUserDto>;

public class GetMeHandler : IRequestHandler<GetMeQuery, LoginUserDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMeHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<LoginUserDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var name = user?.Identity?.Name ?? string.Empty;
        var role = user?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        int.TryParse(idClaim, out var id);

        return Task.FromResult(new LoginUserDto(id, name, role));
    }
}
