using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;

namespace MangaUpdater.Services.Fetcher.Features.Scrapers;

[RegisterScoped]
public sealed partial class AsuraScansScrapper : IFetcher
{
    private readonly HttpClient _httpClient;
    private readonly List<ChapterResult> _chapterList = [];
    
    public AsuraScansScrapper(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient();
    }
    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken)
    {
        var html = await _httpClient.GetStringAsync(request.FullUrl, cancellationToken);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
            
        var chapterNodes = htmlDoc.DocumentNode
            .SelectNodes("//div/h3/a[contains(., 'Chapter')]/ancestor::div[1]")
            .Descendants("h3")
            .Where(h3 => h3.Descendants("a").Any(a => a.InnerText.Contains("Chapter")));
        
        ProcessApiResult(request, chapterNodes);

        return _chapterList;
    }

    private void ProcessApiResult(ChapterQueueMessageDto request, IEnumerable<HtmlNode> nodes)
    {
        foreach (var chapterNode in nodes)
        {
            var chapterNumberString = chapterNode.Descendants("a")
                .First()
                .GetAttributeValue("href", "")
                .Split('/')
                .LastOrDefault()?
                .Trim() ?? throw new InvalidOperationException("Chapter number is invalid.");
            
            var chapterNumber = ExtractNumberFromString(chapterNumberString);
            if (chapterNumber <= request.LastChapterNumber) break;

            var chapterDateString = chapterNode.NextSibling?.InnerText.Trim() 
                                    ?? throw new InvalidOperationException("Chapter date is invalid.");

            var parsedDate = ParseDate(chapterDateString);
            var chapterDate = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0, DateTimeKind.Utc);

            var chapter = new ChapterResult(
                request.MangaId, 
                (int)request.Source,
                chapterNumber.ToString(CultureInfo.InvariantCulture),
                DateTime.SpecifyKind(chapterDate, DateTimeKind.Utc));

            _chapterList.Add(chapter);
        }
    }
    
    private static DateTime ParseDate(string dateString)
    {
        const string format = "MMMM d yyyy";
        var cleanedDateString = DateParseRegex().Replace(dateString, "$1");

        if (DateTime.TryParseExact(cleanedDateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return parsedDate;
        }
        
        throw new FormatException("Invalid date format.");
    }

    private static float ExtractNumberFromString(string input)
    {
        var match = MyRegex().Match(input);

        if (!match.Success) return 0;

        var numericPart = match.Groups[1].Value;

        if (float.TryParse(numericPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatResult))
            return floatResult;
        if (int.TryParse(numericPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intResult))
            return intResult;

        throw new InvalidOperationException("Failed to parse the numeric part as either float or int.");
    }

    [GeneratedRegex(@"(\d+(\.\d+)?)")]
    private static partial Regex MyRegex();
    
    [GeneratedRegex(@"(\d+)(st|nd|rd|th)")]
    private static partial Regex DateParseRegex();
}