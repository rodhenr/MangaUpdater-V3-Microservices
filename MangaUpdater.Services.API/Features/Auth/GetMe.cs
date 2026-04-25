using Microsoft.AspNetCore.WebUtilities;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.API.Features.Auth;

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
        var auth = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        
        if (!string.IsNullOrEmpty(auth)) 
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", auth);
        
        var resp = await client.GetAsync("api/auth/me", cancellationToken);

        if (!resp.IsSuccessStatusCode)
        {
            var message = await resp.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpResponseException(
                resp.StatusCode,
                string.IsNullOrWhiteSpace(message)
                    ? resp.ReasonPhrase ?? ReasonPhrases.GetReasonPhrase((int)resp.StatusCode)
                    : message);
        }
        
        var dto = await resp.Content.ReadFromJsonAsync<LoginUserDto>(cancellationToken: cancellationToken);
        return dto;
    }
}
