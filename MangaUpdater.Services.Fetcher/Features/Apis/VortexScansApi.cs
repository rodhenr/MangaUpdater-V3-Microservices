using System.Globalization;
using MangaUpdater.Services.Fetcher.Extensions;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;

namespace MangaUpdater.Services.Fetcher.Features.Apis;

public class VortexScansApi : IFetcher
{
    private const int TakeParam = 200;
    private readonly HttpClient _httpClient;
    private readonly List<ChapterResult> _chapterList = [];
    
    public VortexScansApi(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient();
    }
    
    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken = default)
    {
        var offset = 0;

        while (true)
        {
            var result = await GetApiResult(request, offset, cancellationToken);

            if (result is null || result.Post.Chapters.Count == 0) break;

            ProcessApiResult(request, result.Post.Chapters);
            offset += TakeParam;
        }

        return _chapterList
            .GroupBy(x => decimal.Parse(x.Number, CultureInfo.InvariantCulture))
            .Select(x => x.OrderBy(y => y.Date).First())
            .ToList();
    }
    
    private async Task<VortexScansDto?> GetApiResult(ChapterQueueMessageDto request, int offset, CancellationToken cancellationToken)
    {   
        var url = $"{request.FullUrl}&skip={offset}&take={TakeParam}&order=desc";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to retrieve data for MangaId `{request.MangaId}` from MangaDex");
        }

        return await response.Content.TryToReadJsonAsync<VortexScansDto>();
    }
    
    private void ProcessApiResult(ChapterQueueMessageDto request, List<VortexScansChapter> apiData)
    {
        var response = apiData
            .Select(c => new
            {
                ChapterNumber = c.Number,
                c.CreatedAt
            })
            .Where(x => x.ChapterNumber > request.LastChapterNumber);
        
        foreach (var chapter in response)
        {
            _chapterList.Add(new ChapterResult(
                request.MangaId,
                (int)request.Source,
                chapter.ChapterNumber.ToString(CultureInfo.InvariantCulture),
                DateTime.SpecifyKind(chapter.CreatedAt, DateTimeKind.Utc)
            ));
        }
    }
}