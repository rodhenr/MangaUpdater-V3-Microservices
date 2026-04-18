using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Extensions;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Features.Scrapers;

[RegisterScoped]
public sealed partial class AsuraScansScrapper : IFetcher
{
    private readonly HttpClient _httpClient;
    private readonly IAppLogger _appLogger;

    public AsuraScansScrapper(IHttpClientFactory clientFactory, IAppLogger appLogger)
    {
        _appLogger = appLogger;
        _httpClient = clientFactory.CreateClient();
    }

    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var lastChapterDecimal = request.LastChapterNumber.GetNumericPart();
            var html = await _httpClient.GetStringAsync($"{request.BaseUrlPart}{request.MangaUrlPart}",
                cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Select all chapter links from the page
            var chapterNodes = htmlDoc.DocumentNode
                .SelectNodes("//a[contains(@href, '/chapter/')]");

            var results = new List<ChapterResult>();

            foreach (var node in chapterNodes)
            {
                var href = node.GetAttributeValue("href", string.Empty);
                if (string.IsNullOrWhiteSpace(href))
                    continue;
                
                // Extract chapter number from URL
                var match = AsuraScansRegex().Match(href);
                if (!match.Success)
                    continue;

                var chapterNumberStr = match.Groups[1].Value;

                // Ignore navigation links like "First Chapter" and "Latest Chapter"
                if (node.InnerHtml.Contains("First Chapter") ||  node.InnerHtml.Contains("Latest Chapter"))
                    continue;
                
                if (!decimal.TryParse(chapterNumberStr, NumberStyles.Any, CultureInfo.InvariantCulture,
                        out var chapterNumber))
                    continue;

                if (chapterNumber <= lastChapterDecimal)
                    continue;

                // Extract relative date (e.g., "1 day ago")
                var dateNode = node.SelectNodes(".//span")?.LastOrDefault();
                var relativeDate = dateNode?.InnerText?.Trim();

                var publishedAt = ParseRelativeDate(relativeDate);

                results.Add(new ChapterResult(
                    request.MangaId,
                    request.MangaName,
                    (int)request.Source,
                    chapterNumberStr,
                    publishedAt,
                    $"{request.BaseUrlPart}{href}"
                ));
            }

            return results
                .GroupBy(x => x.Number)
                .Select(g => g.First())
                .ToList();
        }
        catch (Exception ex)
        {
            _appLogger.LogError(
                "Fetch",
                $"An error occurred while scraping the manga '{request.MangaName}' from URL '{request.BaseUrlPart}{request.MangaUrlPart}'.",
                ex
            );

            return [];
        }
    }

    // Parses strings like "1 day ago", "3 hours ago", etc.
    private static DateTime ParseRelativeDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DateTime.UtcNow;

        var text = value.Trim().ToLowerInvariant();

        // Handle absolute date like "Jan 29, 2024"
        if (DateTime.TryParseExact(
            text,
            "MMM dd, yyyy",
            CultureInfo.GetCultureInfo("en-US"),
            DateTimeStyles.AssumeUniversal,
            out var absoluteDate))
        {
            return DateTime.SpecifyKind(absoluteDate, DateTimeKind.Utc);
        }

        // Handle special keywords
        switch (text)
        {
            case "today":
                return DateTime.UtcNow;
            case "yesterday":
                return DateTime.UtcNow.AddDays(-1);
            case "last week":
                return DateTime.UtcNow.AddDays(-7);
            case "last month":
                return DateTime.UtcNow.AddMonths(-1);
            case "last year":
                return DateTime.UtcNow.AddYears(-1);
        }

        // Handle "x time ago"
        var match = AsuraScansDateRegex().Match(text);

        if (!match.Success) return DateTime.UtcNow;
        
        var valueInt = int.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value;

        return unit switch
        {
            "minute" => DateTime.UtcNow.AddMinutes(-valueInt),
            "hour" => DateTime.UtcNow.AddHours(-valueInt),
            "day" => DateTime.UtcNow.AddDays(-valueInt),
            "week" => DateTime.UtcNow.AddDays(-7 * valueInt),
            "month" => DateTime.UtcNow.AddMonths(-valueInt),
            "year" => DateTime.UtcNow.AddYears(-valueInt),
            _ => DateTime.UtcNow
        };
    }

    [GeneratedRegex(@"chapter/(\d+(\.\d+)?)")]
    private static partial Regex AsuraScansRegex();

    [GeneratedRegex(@"(\d+)\s+(minute|hour|day|week|month|year)s?\s+ago")]
    private static partial Regex AsuraScansDateRegex();
}