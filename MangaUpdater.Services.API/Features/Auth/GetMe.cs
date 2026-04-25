using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.API.Features.Auth;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponseDto?>;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto?>
{
    private readonly IHttpClientFactory _httpFactory;

    public LoginHandler(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<LoginResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var client = _httpFactory.CreateClient("Database");
        var resp = await client.PostAsJsonAsync("api/auth/login", request.Request, cancellationToken);
        resp.EnsureSuccessStatusCode();
        var dto = await resp.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);
        return dto;
    }
}

public record GetMeQuery() : IRequest<LoginUserDto?>;

public class GetMeHandler : IRequestHandler<GetMeQuery, LoginUserDto?>
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMeHandler(IHttpClientFactory httpFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpFactory = httpFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginUserDto?> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var client = _httpFactory.CreateClient("Database");
        var auth = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(auth)) 
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", auth);
        
        var resp = await client.GetAsync("api/auth/me", cancellationToken);
        resp.EnsureSuccessStatusCode();
        
        var dto = await resp.Content.ReadFromJsonAsync<LoginUserDto>(cancellationToken: cancellationToken);
        return dto;
    }
}
