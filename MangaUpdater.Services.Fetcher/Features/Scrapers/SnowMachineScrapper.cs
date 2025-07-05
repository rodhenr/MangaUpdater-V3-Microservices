using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaUpdater.Services.Fetcher.Extensions;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Extensions;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Features.Scrapers;

public partial class SnowMachineScrapper : IFetcher
{
    private readonly HttpClient _httpClient;
    private readonly IAppLogger _appLogger;
    
    public SnowMachineScrapper(IHttpClientFactory clientFactory, IAppLogger appLogger)
    {
        _appLogger = appLogger;
        _httpClient = clientFactory.CreateClient();
    }

    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var lastChapterDecimal = request.LastChapterNumber.GetNumericPart();
            
            var html = await _httpClient.GetStringAsync($"{request.BaseUrlPart}{request.MangaUrlPart}", cancellationToken);
            
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//li[a[contains(text(), 'Chapter')]]");
            var chapterList = new List<ChapterResult>();

            foreach (var node in nodes)
            {
                var chapterNode = node.SelectSingleNode(".//a[contains(text(), 'Chapter')]");
                var chapterText = chapterNode?.InnerText.Trim();
                var chapterNumber = SnowMachineChapterRegex().Match(chapterText ?? string.Empty).Value;
                
                var href = chapterNode.GetAttributeValue("href", string.Empty).Trim();
                var url = $"https://www.snowmtl.ru{href}";

                var dateNode = node.SelectSingleNode(".//span");
                var dateString = dateNode?.InnerText.Trim();
                var chapterDate = ParseDate(dateString);

                chapterList.Add(new ChapterResult(request.MangaId, request.MangaName, (int)SourcesEnum.SnowMachine, chapterNumber, chapterDate,url));
            }
            
            return chapterList
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
    
    private static DateTime ParseDate(string dateText)
    {
        if (string.IsNullOrWhiteSpace(dateText)) throw new ArgumentException("Date text cannot be null or empty.");

        return dateText.Contains("ago", StringComparison.OrdinalIgnoreCase) 
            ? ParseRelativeDate(dateText) 
            : DateTime.Parse(dateText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }

    private static DateTime ParseRelativeDate(string relativeDate)
    {
        var now = DateTime.UtcNow;

        var match = SnowMachineDateRegex().Match(relativeDate);
        if (!match.Success) throw new FormatException($"Relative date format not recognized: {relativeDate}");

        var value = int.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value.ToLower();

        return unit switch
        {
            "second" or "seconds" => now.AddSeconds(-value),
            "minute" or "minutes" => now.AddMinutes(-value),
            "hour" or "hours" => now.AddHours(-value),
            "day" or "days" => now.AddDays(-value),
            "week" or "weeks" => now.AddDays(-7 * value),
            "month" or "months" => now.AddMonths(-value),
            "year" or "years" => now.AddYears(-value),
            _ => throw new NotSupportedException($"Time unit '{unit}' is not supported.")
        };
    }

    [GeneratedRegex(@"\d+(\.\d+)?")]
    private static partial Regex SnowMachineChapterRegex();
    
    [GeneratedRegex(@"(\d+)\s+(second|seconds|minute|minutes|hour|hours|day|days|week|weeks|month|months|year|years)\s+ago", RegexOptions.IgnoreCase, "pt-BR")]
    private static partial Regex SnowMachineDateRegex();
}