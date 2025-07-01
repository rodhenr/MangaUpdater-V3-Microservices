using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Extensions;
using MangaUpdater.Shared.Interfaces;
using PuppeteerSharp;

namespace MangaUpdater.Services.Fetcher.Features.Scrapers;

public partial class ComickScrapper : IFetcher
{
    private readonly IAppLogger _appLogger;
    private readonly IBrowser _browser;
    private readonly List<ChapterResult> _chapterList = [];

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    
    public ComickScrapper(IAppLogger appLogger, IBrowser browser)
    {
        _appLogger = appLogger;
        _browser = browser;
    }

    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken = default)
    {
        var lastChapterDecimal = request.LastChapterNumber.GetNumericPart();
        var page = 1;

        try
        {
            while (true)
            {
                ComickDto? chaptersJson = null;
                
                await using var pageContext = await _browser.NewPageAsync();
                await pageContext.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:140.0) Gecko/20100101 Firefox/140.0");
                await pageContext.SetJavaScriptEnabledAsync(true);
                
                pageContext.Response += async (_, e) =>
                {
                    if (!e.Response.Url.Contains("/chapters?")) return;
                    
                    var body = await e.Response.TextAsync();
                    chaptersJson = JsonSerializer.Deserialize<ComickDto>(body, _jsonSerializerOptions);
                };
                
                var url = $"{request.BaseUrlPart}{request.AditionalInfo}/chapters?lang=en&limit=300&page={page}";
                await pageContext.GoToAsync(url, WaitUntilNavigation.Networkidle2);
                await Task.Delay(2000, cancellationToken);
                
                if (chaptersJson is null || chaptersJson?.Chapters.Count == 0) break;

                ProcessApiResult(request, chaptersJson, lastChapterDecimal);
                page++;
            }

            return _chapterList
                .GroupBy(x => x.Number)
                .Where(chapter => chapter.Key.GetNumericPart() > lastChapterDecimal)
                .Select(x => x.OrderBy(y => y.Number.GetNumericPart()).First())
                .ToList();
        }
        catch (Exception e)
        {
            _appLogger.LogError("Fetch", $"An error occurred while scraping the manga '{request.MangaName}' from URL '{request.BaseUrlPart}{request.MangaUrlPart}'.", e);
            return [];
        }
    }

    private void ProcessApiResult(ChapterQueueMessageDto request, ComickDto apiData, decimal lastChapterDecimal)
    {
        var response = apiData.Chapters
            .Where(c => !string.IsNullOrEmpty(c.Chap) && c.Chap.GetNumericPart() > lastChapterDecimal)
            .Select(c => new
            {
                Id = c.Hid,
                ChapterNumber = c.Chap,
                c.CreatedAt
            });

        foreach (var chapter in response)
        {
            _chapterList.Add(new ChapterResult(
                request.MangaId,
                (int)request.Source,
                chapter.ChapterNumber.ToString(CultureInfo.InvariantCulture),
                chapter.CreatedAt,
                $"https://comick.io/comic/{request.MangaUrlPart}/{chapter.Id}"
            ));
        }
    }

    [GeneratedRegex(@"Chapter\s+([0-9]+(?:\.[0-9]+)?[a-zA-Z]?)")]
    private static partial Regex ComickChapterNumberRegex();
}
