using System.Globalization;
using System.Text.Json;
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
    
    public async Task<List<ChapterResult>> GetChaptersAsync(ChapterQueueMessageDto request, CancellationToken cancellationToken)
    {
        try
        {
            var lastChapterDecimal = request.LastChapterNumber.GetNumericPart();
            var html = await _httpClient.GetStringAsync($"{request.BaseUrlPart}{request.MangaUrlPart}", cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var scriptNodes = htmlDoc.DocumentNode
                .Descendants("script");

            AsuraScansDto? chaptersFinal = null;
            
            foreach (var scriptNode in scriptNodes.Where(x => !string.IsNullOrWhiteSpace(x.InnerHtml)))
            {
                var scriptContent = scriptNode.InnerHtml;
                var match = AsuraScansRegex().Match(scriptContent);

                if (!match.Success) continue;

                var chaptersJson = match.Groups[0].Value;
                var jsonChapters = chaptersJson.Replace("\\\\\"", "\\\"").Replace("\\\"", "\"");
                var finalJson = string.Concat("{", jsonChapters, "}");
                chaptersFinal = JsonSerializer.Deserialize<AsuraScansDto>(finalJson);
            }

            return chaptersFinal?.Chapters
                .GroupBy(chapter => chapter.Number)
                .Where(chapter => chapter.Key > lastChapterDecimal)
                .Select(group =>
                {
                    var chapter = group.OrderBy(x => x.PublishedAt).First();
                    
                    return new ChapterResult(
                        request.MangaId,
                        request.MangaName,
                        (int)request.Source,
                        chapter.Number.ToString("G", CultureInfo.InvariantCulture),
                        DateTime.SpecifyKind(chapter.PublishedAt, DateTimeKind.Utc),
                        $"{request.BaseUrlPart}/{request.MangaUrlPart}/chapter/{chapter.Number}"
                    );
                })
                .ToList() ?? [];
        } 
        catch (Exception ex)
        {
            _appLogger.LogError("Fetch", $"An error occurred while scraping the manga '{request.MangaName}' from URL '{request.BaseUrlPart}{request.MangaUrlPart}'.", ex);
            return [];
        }
    }

    [GeneratedRegex("""\\\"chapters\\\":\[(\{.*?\})\]""", RegexOptions.Singleline)]
    private static partial Regex AsuraScansRegex();
}