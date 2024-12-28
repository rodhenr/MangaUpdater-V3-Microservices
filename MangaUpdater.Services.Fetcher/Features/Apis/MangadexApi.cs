using System.Globalization;
using MangaUpdater.Services.Fetcher.Extensions;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;

namespace MangaUpdater.Services.Fetcher.Features.Apis;

public class MangadexApi : IFetcher
{
    private const string ApiOptions = "feed?translatedLanguage[]=en&limit=199&order[chapter]=asc&limit=200&offset=";
    private readonly HttpClient _httpClient;
    private readonly List<ChapterResult> _chapterList = [];
    
    public MangadexApi(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MangaUpdater/1.0");
    }
    
    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken)
    {
        var offset = 0;

        while (true)
        {
            var result = await GetApiResult(request, offset, cancellationToken);

            if (result is null || result.Data.Count == 0) break;

            ProcessApiResult(request, result.Data);
            offset += 200;
        }

        return _chapterList
            .GroupBy(x => decimal.Parse(x.Number, CultureInfo.InvariantCulture))
            .Select(x => x.OrderBy(y => y.Date).First())
            .ToList();
    }

    private async Task<MangaDexDto?> GetApiResult(ChapterQueueMessageDto request, int offset, CancellationToken cancellationToken)
    {
        var url = $"{request.FullUrl}/{ApiOptions}{offset}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to retrieve data for MangaId `{request.MangaId}` from MangaDex");
        }

        return await response.Content.TryToReadJsonAsync<MangaDexDto>();
    }

    private void ProcessApiResult(ChapterQueueMessageDto request, List<MangaDexResponse> apiData)
    {
        var response = apiData
            .Select(c => new
            {
                ChapterNumber = decimal.Parse(c.Attributes.Chapter, CultureInfo.InvariantCulture),
                c.Attributes.CreatedAt
            })
            .Where(x => x.ChapterNumber > request.LastChapterNumber);
        
        foreach (var chapter in response)
        {
            _chapterList.Add(new ChapterResult(
                request.MangaId,
                (int)request.Source,
                chapter.ChapterNumber.ToString(CultureInfo.InvariantCulture),
                DateTime.SpecifyKind(DateTime.Parse(chapter.CreatedAt), DateTimeKind.Utc),
                ""
            ));
        }
    }
}