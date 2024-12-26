using System.Text.Json;
using MangaUpdater.Services.API.DTOs;
using MangaUpdater.Shared.DTOs;
using MediatR;

namespace MangaUpdater.Services.API.Features.Info;

public record UserChaptersQuery(string Username) : IRequest<List<UserChaptersDto>>;

public class UserChaptersHandler : IRequestHandler<UserChaptersQuery, List<UserChaptersDto>>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserChaptersHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<UserChaptersDto>> Handle(UserChaptersQuery request, CancellationToken cancellationToken)
    {
        var anilistUserData = await GetUserDataFromAnilist(request, cancellationToken);
        var mangasInfo = await GetMangasInfo(request, cancellationToken);

        var response = new List<UserChaptersDto>();

        foreach (var manga in anilistUserData)
        {
            var mangaInfo = mangasInfo.FirstOrDefault(x => x.AniListId == manga.IdAnilist || x.MyAnimeListId == manga.IdMyAnimeList);

            if (mangaInfo is null) continue;

            var lastChapterFromSource = mangaInfo.Chapters.Max(x => x.Number);
            
            var info = new UserChaptersDto(
                manga.IdMyAnimeList,
                manga.UrlAnilist,
                manga.IdAnilist,
                manga.UrlAnilist,
                mangaInfo.TitleRomaji,
                mangaInfo.TitleEnglish,
                lastChapterFromSource >= manga.UserLastChapterRead,
                lastChapterFromSource,
                manga.UserLastChapterRead
            );
            
            response.Add(info);
        }

        return response;
    }

    private async Task<List<AnilistUserMangaCollectionDto>> GetUserDataFromAnilist(UserChaptersQuery request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        
        var response = await client.GetAsync($"https://localhost:7281/api/user/{request.Username}", cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<List<AnilistUserMangaCollectionDto>>(content, _jsonOptions);

        return data ?? [];
    }

    private async Task<List<MangaChaptersDto>> GetMangasInfo(UserChaptersQuery request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        
        var response = await client.GetAsync($"https://localhost:7142/api/manga", cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<List<MangaChaptersDto>>(content, _jsonOptions);

        return data ?? [];
    }
}