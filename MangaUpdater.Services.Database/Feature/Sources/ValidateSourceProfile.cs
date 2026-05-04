using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record ValidateSourceProfileQuery(int SourceId, ValidateSourceProfileRequest Request)
    : IRequest<SourceProfileValidationResultDto>;

public class ValidateSourceProfileHandler : IRequestHandler<ValidateSourceProfileQuery, SourceProfileValidationResultDto>
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public ValidateSourceProfileHandler(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<SourceProfileValidationResultDto> Handle(ValidateSourceProfileQuery request,
        CancellationToken cancellationToken)
    {
        var source = await SourceConfigurationSupport.LoadSourceGraphAsync(_context, request.SourceId, cancellationToken);
        var targetUrl = SourceConfigurationSupport.NormalizeRequiredValue(request.Request.TargetUrl, "TargetUrl");
        var previewLimit = request.Request.PreviewLimit <= 0 ? 20 : request.Request.PreviewLimit;
        var warnings = new List<string>();
        var diagnostics = new List<string>();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, targetUrl);
        ApplyRequestProfileHeaders(httpRequest, source, request.Request.RequestProfileId);

        using var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        IReadOnlyCollection<GenericChapterCandidate> preview;

        if (string.Equals(source.EngineType, "HtmlXPath", StringComparison.OrdinalIgnoreCase))
        {
            var scrapingProfile = SourceConfigurationSupport.SelectScrapingProfile(source, request.Request.ScrapingProfileId)
                                 ?? throw new HttpResponseException(HttpStatusCode.BadRequest,
                                     "No scraping profile was found for this source.");

            preview = PreviewHtml(source, scrapingProfile, content, previewLimit, warnings, diagnostics);
        }
        else if (string.Equals(source.EngineType, "JsonApi", StringComparison.OrdinalIgnoreCase))
        {
            var apiProfile = SourceConfigurationSupport.SelectApiProfile(source, request.Request.ApiProfileId)
                             ?? throw new HttpResponseException(HttpStatusCode.BadRequest,
                                 "No API profile was found for this source.");

            preview = PreviewJson(source, apiProfile, content, previewLimit, warnings, diagnostics);
        }
        else
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest,
                $"Validation preview is not supported for engine type '{source.EngineType}'.");
        }

        return new SourceProfileValidationResultDto(source.EngineType, targetUrl, preview, warnings, diagnostics);
    }

    private static void ApplyRequestProfileHeaders(HttpRequestMessage httpRequest, Source source, int? requestProfileId)
    {
        var requestProfile = SourceConfigurationSupport.SelectRequestProfile(source, requestProfileId);
        if (requestProfile is null)
            return;

        if (!string.IsNullOrWhiteSpace(requestProfile.AcceptLanguage))
            httpRequest.Headers.TryAddWithoutValidation("Accept-Language", requestProfile.AcceptLanguage);

        if (!string.IsNullOrWhiteSpace(requestProfile.Referrer))
            httpRequest.Headers.Referrer = new Uri(requestProfile.Referrer, UriKind.Absolute);

        if (string.IsNullOrWhiteSpace(requestProfile.HeadersJson))
            return;

        try
        {
            var headers = JsonSerializer.Deserialize<Dictionary<string, string?>>(requestProfile.HeadersJson);
            if (headers is null)
                return;

            foreach (var header in headers.Where(header => !string.IsNullOrWhiteSpace(header.Value)))
                httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        catch
        {
            // Invalid header json is surfaced later as a diagnostics gap in the preview result.
        }
    }

    private static List<GenericChapterCandidate> PreviewHtml(Source source, SourceScrapingProfile profile,
        string html, int previewLimit, List<string> warnings, List<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(profile.ChapterNodesXPath))
            throw new HttpResponseException(HttpStatusCode.BadRequest, "ChapterNodesXPath is required for HtmlXPath validation.");

        var document = new HtmlDocument();
        document.LoadHtml(html);
        var nodes = document.DocumentNode.SelectNodes(profile.ChapterNodesXPath)?.ToList() ?? [];

        if (nodes.Count == 0)
        {
            warnings.Add($"No chapter nodes were found for xpath '{profile.ChapterNodesXPath}'.");
            return [];
        }

        var parsingProfile = BuildHtmlProfile(profile);
        var candidates = new List<GenericChapterCandidate>();
        var extractedNumbers = 0;

        foreach (var node in nodes)
        {
            if (ShouldIgnoreNode(node, parsingProfile))
                continue;

            var rawUrl = ExtractNodeValue(node, profile.ChapterUrlXPath, profile.ChapterUrlAttribute);
            if (string.IsNullOrWhiteSpace(rawUrl))
                continue;

            var numberSource = ExtractNodeValue(node, profile.ChapterNumberXPath, profile.ChapterNumberAttribute) ?? rawUrl;
            var number = NormalizeChapterNumber(numberSource, profile.ChapterNumberRegex);
            if (string.IsNullOrWhiteSpace(number))
                continue;

            extractedNumbers++;
            var rawDate = ExtractNodeValue(node, profile.ChapterDateXPath, profile.ChapterDateAttribute);
            var parsedDate = ParseHtmlDate(rawDate, parsingProfile, warnings);
            var normalizedUrl = NormalizeUrl(rawUrl, source.BaseUrl, parsingProfile);
            candidates.Add(new GenericChapterCandidate(number, parsedDate, normalizedUrl, node.InnerText.Trim()));
        }

        if (extractedNumbers == 0)
            diagnostics.Add($"XPath matched {nodes.Count} nodes but no chapter number could be extracted.");

        return candidates
            .GroupBy(candidate => candidate.Number)
            .Select(group => group.OrderByDescending(candidate => candidate.Date).First())
            .Take(previewLimit)
            .ToList();
    }

    private static IReadOnlyCollection<GenericChapterCandidate> PreviewJson(Source source, SourceApiProfile profile,
        string json, int previewLimit, ICollection<string> warnings, ICollection<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(profile.DataRootPath))
            throw new HttpResponseException(HttpStatusCode.BadRequest, "DataRootPath is required for JsonApi validation.");

        if (string.IsNullOrWhiteSpace(profile.ChapterNumberPath))
            throw new HttpResponseException(HttpStatusCode.BadRequest, "ChapterNumberPath is required for JsonApi validation.");

        using var document = JsonDocument.Parse(json);
        var items = ExtractJsonArray(document.RootElement, profile.DataRootPath);

        if (items.Count == 0)
        {
            warnings.Add($"JSON path '{profile.DataRootPath}' did not resolve to any array items.");
            return Array.Empty<GenericChapterCandidate>();
        }

        var preview = new List<GenericChapterCandidate>();
        foreach (var item in items.Take(previewLimit))
        {
            var number = ExtractJsonString(item, profile.ChapterNumberPath);
            if (string.IsNullOrWhiteSpace(number))
                continue;

            var rawDate = ExtractJsonString(item, profile.ChapterDatePath);
            var urlValue = ExtractJsonString(item, profile.ChapterUrlPath);
            preview.Add(new GenericChapterCandidate(
                number,
                ParseJsonDate(rawDate, warnings),
                ResolveResultUrl(urlValue, profile, source.BaseUrl),
                item.GetRawText()));
        }

        if (preview.Count == 0)
            diagnostics.Add($"JSON path '{profile.DataRootPath}' resolved items, but no chapter numbers were extracted.");

        return preview;
    }

    private static Dictionary<string, string?> BuildHtmlProfile(SourceScrapingProfile profile)
    {
        return new Dictionary<string, string?>
        {
            ["DateParseMode"] = profile.DateParseMode,
            ["DateCulture"] = profile.DateCulture,
            ["DateFormatPrimary"] = profile.DateFormatPrimary,
            ["DateFormatSecondary"] = profile.DateFormatSecondary,
            ["RelativeDateRegex"] = profile.RelativeDateRegex,
            ["IgnoreTextContains1"] = profile.IgnoreTextContains1,
            ["IgnoreTextContains2"] = profile.IgnoreTextContains2,
            ["IgnoreTextContains3"] = profile.IgnoreTextContains3,
            ["UrlPrefix"] = profile.UrlPrefix,
            ["UrlJoinMode"] = profile.UrlJoinMode
        };
    }

    private static string? ExtractNodeValue(HtmlNode node, string? xpath, string? attributeName)
    {
        var targetNode = string.IsNullOrWhiteSpace(xpath) || xpath == "." ? node : node.SelectSingleNode(xpath);
        if (targetNode is null)
            return null;

        if (!string.IsNullOrWhiteSpace(attributeName))
        {
            var attributeValue = targetNode.GetAttributeValue(attributeName, string.Empty)?.Trim();
            return string.IsNullOrWhiteSpace(attributeValue) ? null : attributeValue;
        }

        var innerText = HtmlEntity.DeEntitize(targetNode.InnerText).Trim();
        return string.IsNullOrWhiteSpace(innerText) ? null : innerText;
    }

    private static string? NormalizeChapterNumber(string? rawValue, string? regexPattern)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return null;

        if (string.IsNullOrWhiteSpace(regexPattern))
            return rawValue.Trim();

        var match = Regex.Match(rawValue, regexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!match.Success)
            return null;

        return match.Groups.Count > 1 ? match.Groups[1].Value.Trim() : match.Value.Trim();
    }

    private static bool ShouldIgnoreNode(HtmlNode node, IReadOnlyDictionary<string, string?> parsingProfile)
    {
        var text = HtmlEntity.DeEntitize(node.InnerText);
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return new[]
            {
                parsingProfile.GetValueOrDefault("IgnoreTextContains1"),
                parsingProfile.GetValueOrDefault("IgnoreTextContains2"),
                parsingProfile.GetValueOrDefault("IgnoreTextContains3")
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Any(value => text.Contains(value!, StringComparison.OrdinalIgnoreCase));
    }

    private static DateTime ParseHtmlDate(string? rawValue, IReadOnlyDictionary<string, string?> parsingProfile,
        List<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return DateTime.UtcNow;

        var parseMode = parsingProfile.GetValueOrDefault("DateParseMode");
        var cultureName = parsingProfile.GetValueOrDefault("DateCulture") ?? "en-US";
        var culture = CultureInfo.GetCultureInfo(cultureName);
        var normalizedValue = rawValue.Trim();

        if (string.Equals(parseMode, "IsoDateTime", StringComparison.OrdinalIgnoreCase))
            return DateTime.Parse(normalizedValue, culture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

        if (TryParseRelativeDate(normalizedValue, parsingProfile, out var relativeDate))
            return relativeDate;

        var exactFormats = new[]
        {
            parsingProfile.GetValueOrDefault("DateFormatPrimary"),
            parsingProfile.GetValueOrDefault("DateFormatSecondary")
        }.Where(value => !string.IsNullOrWhiteSpace(value)).Cast<string>().ToArray();

        foreach (var exactFormat in exactFormats)
        {
            if (DateTime.TryParseExact(normalizedValue, exactFormat, culture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var exactDate))
                return exactDate;
        }

        if (LooksLikeRelativeDate(normalizedValue))
        {
            warnings.Add($"Could not parse relative date value '{normalizedValue}', defaulting to current UTC time.");
            return DateTime.UtcNow;
        }

        if (DateTime.TryParse(normalizedValue, culture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsedDate))
            return parsedDate;

        warnings.Add($"Could not parse date value '{normalizedValue}', defaulting to current UTC time.");
        return DateTime.UtcNow;
    }

    private static bool TryParseRelativeDate(string rawValue, IReadOnlyDictionary<string, string?> parsingProfile,
        out DateTime parsedDate)
    {
        var normalizedValue = rawValue.Trim().ToLowerInvariant();
        var now = DateTimeOffset.UtcNow;
        var calendarAnchor = CreateCalendarAnchor(now);

        parsedDate = normalizedValue switch
        {
            "today" => calendarAnchor.UtcDateTime,
            "yesterday" => calendarAnchor.AddDays(-1).UtcDateTime,
            "last week" => calendarAnchor.AddDays(-7).UtcDateTime,
            "last month" => calendarAnchor.AddMonths(-1).UtcDateTime,
            "last year" => calendarAnchor.AddYears(-1).UtcDateTime,
            _ => default
        };

        if (parsedDate != default)
            return true;

        var relativePattern = parsingProfile.GetValueOrDefault("RelativeDateRegex")
                              ?? "(\\d+)\\s+(second|seconds|minute|minutes|hour|hours|day|days|week|weeks|month|months|year|years)\\s+ago";
        var match = Regex.Match(normalizedValue, relativePattern,
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!TryExtractRelativeValueAndUnit(normalizedValue, match, out var value, out var unit))
            return false;

        return TryApplyRelativeOffset(now, value, unit, out parsedDate);
    }

    private static bool TryExtractRelativeValueAndUnit(string normalizedValue, Match configuredMatch,
        out int value, out string unit)
    {
        if (TryExtractConfiguredRelativeValueAndUnit(configuredMatch, out value, out unit))
            return true;

        var numericMatch = Regex.Match(normalizedValue,
            "^(?<value>\\d+)\\s+(?<unit>seconds?|secs?|sec|minutes?|mins?|min|hours?|hrs?|hr|days?|weeks?|wks?|wk|months?|mos?|mo|years?|yrs?|yr)\\s+ago$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        if (numericMatch.Success &&
            int.TryParse(numericMatch.Groups["value"].Value, CultureInfo.InvariantCulture, out value))
        {
            unit = NormalizeRelativeUnit(numericMatch.Groups["unit"].Value);
            return !string.IsNullOrWhiteSpace(unit);
        }

        var articleMatch = Regex.Match(normalizedValue,
            "^(?<value>a|an|one)\\s+(?<unit>second|sec|minute|min|hour|hr|day|week|wk|month|mo|year|yr)\\s+ago$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        if (articleMatch.Success)
        {
            value = 1;
            unit = NormalizeRelativeUnit(articleMatch.Groups["unit"].Value);
            return !string.IsNullOrWhiteSpace(unit);
        }

        value = 0;
        unit = string.Empty;
        return false;
    }

    private static bool TryExtractConfiguredRelativeValueAndUnit(Match configuredMatch, out int value, out string unit)
    {
        if (configuredMatch.Success)
        {
            if (TryParseRelativeValue(configuredMatch.Groups["value"].Value, out value))
            {
                unit = NormalizeRelativeUnit(configuredMatch.Groups["unit"].Value);
                if (!string.IsNullOrWhiteSpace(unit))
                    return true;
            }

            var capturedValues = configuredMatch.Groups
                .Cast<Group>()
                .Skip(1)
                .Where(group => group.Success)
                .Select(group => group.Value.Trim())
                .Where(groupValue => !string.IsNullOrWhiteSpace(groupValue))
                .ToArray();

            for (var index = 0; index < capturedValues.Length; index++)
            {
                if (!TryParseRelativeValue(capturedValues[index], out value))
                    continue;

                for (var unitIndex = index + 1; unitIndex < capturedValues.Length; unitIndex++)
                {
                    unit = NormalizeRelativeUnit(capturedValues[unitIndex]);
                    if (!string.IsNullOrWhiteSpace(unit))
                        return true;
                }
            }
        }

        value = 0;
        unit = string.Empty;
        return false;
    }

    private static bool TryParseRelativeValue(string rawValue, out int value)
    {
        if (int.TryParse(rawValue, CultureInfo.InvariantCulture, out value))
            return true;

        if (string.Equals(rawValue, "a", StringComparison.OrdinalIgnoreCase)
            || string.Equals(rawValue, "an", StringComparison.OrdinalIgnoreCase)
            || string.Equals(rawValue, "one", StringComparison.OrdinalIgnoreCase))
        {
            value = 1;
            return true;
        }

        value = 0;
        return false;
    }

    private static bool TryApplyRelativeOffset(DateTimeOffset now, int value, string unit, out DateTime parsedDate)
    {
        parsedDate = unit switch
        {
            "seconds" => now.AddSeconds(-value).UtcDateTime,
            "minutes" => now.AddMinutes(-value).UtcDateTime,
            "hours" => CreateCalendarAnchor(now.AddHours(-value)).UtcDateTime,
            "days" => CreateCalendarAnchor(now.AddDays(-value)).UtcDateTime,
            "weeks" => CreateCalendarAnchor(now.AddDays(-ConvertWeeksToDays(value))).UtcDateTime,
            "months" => CreateCalendarAnchor(now.AddMonths(-value)).UtcDateTime,
            "years" => CreateCalendarAnchor(now.AddYears(-value)).UtcDateTime,
            _ => default
        };

        return parsedDate != default;
    }

    private static int ConvertWeeksToDays(int weeks)
    {
        return weeks == 4 ? 30 : weeks * 7;
    }

    private static DateTimeOffset CreateCalendarAnchor(DateTimeOffset value)
    {
        return new DateTimeOffset(value.Year, value.Month, value.Day, 12, 0, 0, value.Offset);
    }

    private static string NormalizeRelativeUnit(string rawUnit)
    {
        return rawUnit.Trim().ToLowerInvariant() switch
        {
            "second" or "seconds" or "sec" or "secs" => "seconds",
            "minute" or "minutes" or "min" or "mins" => "minutes",
            "hour" or "hours" or "hr" or "hrs" => "hours",
            "day" or "days" => "days",
            "week" or "weeks" or "wk" or "wks" => "weeks",
            "month" or "months" or "mo" or "mos" => "months",
            "year" or "years" or "yr" or "yrs" => "years",
            _ => string.Empty
        };
    }

    private static bool LooksLikeRelativeDate(string normalizedValue)
    {
        return normalizedValue.Contains("ago", StringComparison.Ordinal)
               || string.Equals(normalizedValue, "today", StringComparison.OrdinalIgnoreCase)
               || string.Equals(normalizedValue, "yesterday", StringComparison.OrdinalIgnoreCase)
               || string.Equals(normalizedValue, "last week", StringComparison.OrdinalIgnoreCase)
               || string.Equals(normalizedValue, "last month", StringComparison.OrdinalIgnoreCase)
               || string.Equals(normalizedValue, "last year", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeUrl(string urlValue, string sourceBaseUrl,
        IReadOnlyDictionary<string, string?> parsingProfile)
    {
        if (Uri.TryCreate(urlValue, UriKind.Absolute, out var absoluteUri))
            return absoluteUri.ToString();

        var urlJoinMode = parsingProfile.GetValueOrDefault("UrlJoinMode");
        var urlPrefix = parsingProfile.GetValueOrDefault("UrlPrefix");

        if (string.Equals(urlJoinMode, "FixedPrefix", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(urlPrefix))
            return $"{urlPrefix.TrimEnd('/')}/{urlValue.TrimStart('/')}";

        return $"{sourceBaseUrl.TrimEnd('/')}/{urlValue.TrimStart('/')}";
    }

    private static List<JsonElement> ExtractJsonArray(JsonElement rootElement, string path)
    {
        var target = TraversePath(rootElement, path);
        if (target is null || target.Value.ValueKind != JsonValueKind.Array)
            return [];

        return target.Value.EnumerateArray().ToList();
    }

    private static string? ExtractJsonString(JsonElement element, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        var value = TraversePath(element, path);
        if (value is null)
            return null;

        return value.Value.ValueKind switch
        {
            JsonValueKind.String => value.Value.GetString(),
            JsonValueKind.Number => value.Value.GetRawText(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            _ => value.Value.GetRawText()
        };
    }

    private static JsonElement? TraversePath(JsonElement rootElement, string path)
    {
        var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var current = rootElement;

        foreach (var segment in segments)
        {
            switch (current.ValueKind)
            {
                case JsonValueKind.Object when !current.TryGetProperty(segment, out current):
                    return null;
                case JsonValueKind.Object:
                    continue;
                case JsonValueKind.Array when int.TryParse(segment, out var index):
                {
                    var items = current.EnumerateArray().ToArray();
                    if (index < 0 || index >= items.Length)
                        return null;

                    current = items[index];
                    continue;
                }
                case JsonValueKind.Undefined:
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                default:
                    return null;
            }
        }

        return current;
    }

    private static DateTime ParseJsonDate(string? rawValue, ICollection<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return DateTime.UtcNow;

        if (DateTime.TryParse(rawValue, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsedDate))
            return parsedDate;

        warnings.Add($"Could not parse JSON API date value '{rawValue}', defaulting to current UTC time.");
        return DateTime.UtcNow;
    }

    private static string ResolveResultUrl(string? rawValue, SourceApiProfile profile, string sourceBaseUrl)
    {
        if (!string.IsNullOrWhiteSpace(profile.ResultUrlTemplate))
            return profile.ResultUrlTemplate.Replace("{Value}", rawValue ?? string.Empty, StringComparison.Ordinal);

        if (string.IsNullOrWhiteSpace(rawValue))
            return string.Empty;

        return Uri.TryCreate(rawValue, UriKind.Absolute, out var absoluteUri) 
            ? absoluteUri.ToString() 
            : $"{sourceBaseUrl.TrimEnd('/')}/{rawValue.TrimStart('/')}";
    }
}