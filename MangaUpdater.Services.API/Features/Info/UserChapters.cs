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
    private readonly string _anilistConnectorMicroserviceUrl;
    private readonly string _databaseMicroserviceUrl;

    public UserChaptersHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _anilistConnectorMicroserviceUrl = configuration["Microservices:AnilistConnector"] ?? throw new Exception("Anilist connector url is not configured.");
        _databaseMicroserviceUrl = configuration["Microservices:Database"] ?? throw new Exception("Database url is not configured.");;
    }

    public async Task<List<UserChaptersDto>> Handle(UserChaptersQuery request, CancellationToken cancellationToken)
    {
        var anilistUserData = await GetUserDataFromAnilist(request, cancellationToken);
        var mangasInfo = await GetMangasInfo(request, cancellationToken);

        var response = new List<UserChaptersDto>();

        foreach (var manga in anilistUserData)
        {
            var mangaInfo = mangasInfo.FirstOrDefault(x => x.AniListId == manga.IdAnilist || x.MyAnimeListId == manga.IdMyAnimeList);
            var lastChapter = mangaInfo?.Chapters.MaxBy(x => x.Number);
            
            var info = new UserChaptersDto(
                manga.IdMyAnimeList,
                manga.UrlMyAnimeList,
                manga.IdAnilist,
                manga.UrlAnilist,
                mangaInfo?.TitleRomaji ?? manga.Title,
                mangaInfo?.TitleEnglish ?? manga.Title,
                lastChapter?.Number,
                lastChapter?.Date,
                manga.UserLastChapterRead,
                manga.CoverImage
            );
            
            response.Add(info);
        }

        return response;
    }

    private async Task<List<AnilistUserMangaCollectionDto>> GetUserDataFromAnilist(UserChaptersQuery request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        
        var response = await client.GetAsync($"{_anilistConnectorMicroserviceUrl}/api/user/{request.Username}", cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<List<AnilistUserMangaCollectionDto>>(content, _jsonOptions);

        return data ?? [];
    }

    private async Task<List<MangaChaptersDto>> GetMangasInfo(UserChaptersQuery request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        
        var response = await client.GetAsync($"{_databaseMicroserviceUrl}/api/manga/full", cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<List<MangaChaptersDto>>(content, _jsonOptions);

        return data ?? [];
    }
}