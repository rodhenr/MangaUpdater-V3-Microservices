using System.Globalization;
using MangaUpdater.Services.Fetcher.Extensions;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;

namespace MangaUpdater.Services.Fetcher.Features.Apis;

public class MangadexApi : IFetcher
{
    private const string ApiOptions = "feed?translatedLanguage[]=en&limit=199&order[chapter]=asc&limit=500&offset=";
    private readonly HttpClient _httpClient;
    private readonly List<ChapterResult> _chapterList = [];
    
    public MangadexApi(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MangaUpdater/1.0");
    }
    
    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterRequest request, CancellationToken cancellationToken)
    {
        var offset = 0;

        while (true)
        {
            var result = await GetApiResult(request, offset, cancellationToken);

            if (result is null || result.Data.Count == 0) break;

            ProcessApiResult(request, result.Data);
            offset += 200;
        }

        return _chapterList;
    }

    private async Task<MangaDexDto?> GetApiResult(ChapterRequest request, int offset, CancellationToken cancellationToken)
    {
        var url = $"{request.FullUrl}/{ApiOptions}{offset}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to retrieve data for MangaId `{request.MangaId}` from MangaDex");
        }

        return await response.Content.TryToReadJsonAsync<MangaDexDto>();
    }

    private void ProcessApiResult(ChapterRequest request, List<MangaDexResponse> apiData)
    {
        var response = apiData
            .Select(chapter => new { chapter, ChapterNumber = float.Parse(chapter.Attributes.Chapter, CultureInfo.InvariantCulture) })
            .Where(x => !(x.ChapterNumber <= request.LastChapterNumber))
            .Select(x => x.chapter);
        
        foreach (var chapter in response)
        {
            _chapterList.Add(new ChapterResult(
                request.MangaId,
                (int)request.Source,
                chapter.Attributes.Chapter,
                DateTime.SpecifyKind(DateTime.Parse(chapter.Attributes.CreatedAt), DateTimeKind.Utc)
            ));
        }
    }
}