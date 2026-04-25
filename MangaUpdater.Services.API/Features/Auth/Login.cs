using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.API.Features.Auth;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResult>;

public record LoginResult(int StatusCode, LoginResponseDto? Payload, string? ErrorContent)
{
    public bool IsSuccess => StatusCode is >= 200 and < 300;
}

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IHttpClientFactory _httpFactory;

    public LoginHandler(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var client = _httpFactory.CreateClient("Database");
        var resp = await client.PostAsJsonAsync("api/auth/login", new { username = request.Request.Username, password = request.Request.Password }, cancellationToken);

        if (!resp.IsSuccessStatusCode)
        {
            return new LoginResult((int)resp.StatusCode, null, "An  error occured.");
        }
        
        var dto = await resp.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);
        return new LoginResult((int)resp.StatusCode, dto, null);
    }
}