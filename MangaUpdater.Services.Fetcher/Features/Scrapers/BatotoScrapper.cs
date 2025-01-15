using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Features.Scrapers;

public partial class BatotoScrapper : IFetcher
{
    private readonly HttpClient _httpClient;
    private readonly IAppLogger _appLogger;
    
    public BatotoScrapper(IHttpClientFactory clientFactory, IAppLogger appLogger)
    {
        _appLogger = appLogger;
        _httpClient = clientFactory.CreateClient();
    }

    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(request.FullUrl, cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[starts-with(@data-hk, '0-0-') and .//a[contains(text(), 'Chapter')]]");
            var chapterList = new List<ChapterResult>();

            foreach (var chapterDiv in nodes)
            {
                var chapterLink = chapterDiv.SelectSingleNode(".//a");
                var chapterText  = chapterLink?.InnerText.Trim();
                var chapterNumber = ExtractChapterNumber(chapterText);
                
                var url = "https://xbato.com" + chapterLink?.GetAttributeValue("href", string.Empty);

                var dateNode = chapterDiv.SelectSingleNode(".//time");
                var dateString = dateNode?.GetAttributeValue("time", string.Empty);
                var chapterDate = DateTime.Parse(dateString, null, DateTimeStyles.RoundtripKind).ToUniversalTime();

                chapterList.Add(new ChapterResult(request.MangaId, (int)SourcesEnum.Batoto, chapterNumber, chapterDate,url));
            }
            
            return chapterList
                .GroupBy(x => decimal.Parse(x.Number, CultureInfo.InvariantCulture))
                .Where(chapter => chapter.Key > request.LastChapterNumber)
                .Select(x => x.OrderBy(y => y.Date).First())
                .ToList();
        }
        catch (Exception e)
        {
            _appLogger.LogError("Fetch", $"An error occurred while scraping the manga '{request.MangaId}' from URL '{request.FullUrl}'.", e);
            return [];
        }
    }
    
    static string ExtractChapterNumber(string chapterText)
    {
        var match = MyRegex().Match(chapterText);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    [GeneratedRegex(@"(?:Volume\s\d+\s)?Chapter\s(\d+(\.\d+)?)", RegexOptions.IgnoreCase, "pt-BR")]
    private static partial Regex MyRegex();
}