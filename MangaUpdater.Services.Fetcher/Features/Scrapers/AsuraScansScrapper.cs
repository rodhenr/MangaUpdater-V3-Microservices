using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Models;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Interfaces;

namespace MangaUpdater.Services.Fetcher.Features.Scrapers;

public class Chapter
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public int Number { get; set; }
    
    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
}

public class Root
{
    [JsonPropertyName("chapters")] 
    public List<Chapter> Chapters { get; set; } = [];
}

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
            var html = await _httpClient.GetStringAsync(request.FullUrl, cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var scriptNodes = htmlDoc.DocumentNode
                .Descendants("script");

            Root? chaptersFinal = null;
            
            foreach (var scriptNode in scriptNodes.Where(x => !string.IsNullOrWhiteSpace(x.InnerHtml)))
            {
                var scriptContent = scriptNode.InnerHtml;
                var match = AsuraScansRegex().Match(scriptContent);

                if (!match.Success) continue;

                var chaptersJson = match.Groups[0].Value;
                var jsonChapters = chaptersJson.Replace("\\\"", "\"");
                var finalJson = string.Concat("{", jsonChapters, "}");
                chaptersFinal = JsonSerializer.Deserialize<Root>(finalJson);
            }

            return chaptersFinal?.Chapters
                .GroupBy(chapter => chapter.Number)
                .Select(group =>
                {
                    var chapter = group.OrderBy(x => x.PublishedAt).First();
                    
                    return new ChapterResult(
                        request.MangaId,
                        (int)request.Source,
                        chapter.Number.ToString(CultureInfo.InvariantCulture),
                        DateTime.SpecifyKind(chapter.PublishedAt, DateTimeKind.Utc)
                    );
                })
                .ToList() ?? [];
        } 
        catch (Exception ex)
        {
            _appLogger.LogError("Fetch", $"An error occurred while scraping the manga '{request.MangaId}' from URL '{request.FullUrl}'.", ex);
            return [];
        }
    }

    [GeneratedRegex("""\\\"chapters\\\":\[(\{.*?\})\]""", RegexOptions.Singleline)]
    private static partial Regex AsuraScansRegex();
}