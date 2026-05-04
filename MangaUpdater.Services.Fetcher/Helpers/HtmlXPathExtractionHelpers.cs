using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace MangaUpdater.Services.Fetcher.Helpers;

internal static class HtmlXPathExtractionHelpers
{
    public static string? GetProfileValue(IReadOnlyDictionary<string, string?> parsingProfile, string key)
    {
        return parsingProfile.GetValueOrDefault(key);
    }

    public static string? GetRequiredProfileValue(IReadOnlyDictionary<string, string?> parsingProfile, string key)
    {
        return GetProfileValue(parsingProfile, key);
    }

    public static string BuildRequestUrl(MangaUpdater.Shared.Models.SourceRuntimeDefinition runtimeDefinition,
        MangaUpdater.Shared.Models.GenericScraperContext context)
    {
        var template = runtimeDefinition.RequestProfile.GetValueOrDefault("UrlTemplate");
        if (string.IsNullOrWhiteSpace(template))
            return $"{context.SourceBaseUrl}{context.MangaPath}";

        return template
            .Replace("{BaseUrl}", context.SourceBaseUrl, StringComparison.Ordinal)
            .Replace("{SourceBaseUrl}", context.SourceBaseUrl, StringComparison.Ordinal)
            .Replace("{MangaPath}", context.MangaPath, StringComparison.Ordinal)
            .Replace("{MangaUrlPart}", context.MangaPath, StringComparison.Ordinal)
            .Replace("{AdditionalInfo}", context.AdditionalInfo ?? string.Empty, StringComparison.Ordinal);
    }

    public static string? ExtractNodeValue(HtmlNode node, string? xpath, string? attributeName)
    {
        var targetNode = ResolveTargetNode(node, xpath);
        if (targetNode is null)
            return null;

        if (!string.IsNullOrWhiteSpace(attributeName))
        {
            var attributeValue = targetNode.GetAttributeValue(attributeName, string.Empty)?.Trim();
            return string.IsNullOrWhiteSpace(attributeValue) ? null : attributeValue;
        }

        var innerText = ExtractInnerTextWithSpacing(targetNode);
        return string.IsNullOrWhiteSpace(innerText) ? null : innerText;
    }

    public static string? NormalizeChapterNumber(string? rawValue, string? regexPattern)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return null;

        if (string.IsNullOrWhiteSpace(regexPattern))
            return rawValue.Trim();

        var match = Regex.Match(rawValue, regexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!match.Success)
            return null;

        if (match.Groups.Count > 1)
            return match.Groups[1].Value.Trim();

        return match.Value.Trim();
    }

    public static DateTime ParseDate(string? rawValue, IReadOnlyDictionary<string, string?> parsingProfile,
        ICollection<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return DateTime.UtcNow;

        var parseMode = GetProfileValue(parsingProfile, "DateParseMode");
        var cultureName = GetProfileValue(parsingProfile, "DateCulture") ?? "en-US";
        var culture = CultureInfo.GetCultureInfo(cultureName);
        var normalizedValue = ExtractDateValue(rawValue.Trim(), parsingProfile);

        if (string.Equals(parseMode, "IsoDateTime", StringComparison.OrdinalIgnoreCase))
            return DateTime.Parse(normalizedValue, culture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

        if (TryParseRelativeDate(normalizedValue, parsingProfile, out var relativeDate))
            return relativeDate;

        var exactFormats = new[]
        {
            GetProfileValue(parsingProfile, "DateFormatPrimary"),
            GetProfileValue(parsingProfile, "DateFormatSecondary")
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

    public static string NormalizeUrl(string urlValue, string sourceBaseUrl,
        IReadOnlyDictionary<string, string?> parsingProfile)
    {
        if (Uri.TryCreate(urlValue, UriKind.Absolute, out var absoluteUri) &&
            (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps))
            return absoluteUri.ToString();

        var urlJoinMode = GetProfileValue(parsingProfile, "UrlJoinMode");
        var urlPrefix = GetProfileValue(parsingProfile, "UrlPrefix");

        if (string.Equals(urlJoinMode, "FixedPrefix", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(urlPrefix))
            return $"{urlPrefix.TrimEnd('/')}/{urlValue.TrimStart('/')}";

        return $"{sourceBaseUrl.TrimEnd('/')}/{urlValue.TrimStart('/')}";
    }

    public static bool ShouldIgnoreNode(HtmlNode node, IReadOnlyDictionary<string, string?> parsingProfile)
    {
        var text = HtmlEntity.DeEntitize(node.InnerText);
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var ignoreValues = new[]
        {
            GetProfileValue(parsingProfile, "IgnoreTextContains1"),
            GetProfileValue(parsingProfile, "IgnoreTextContains2"),
            GetProfileValue(parsingProfile, "IgnoreTextContains3")
        };

        return ignoreValues
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Any(value => text.Contains(value!, StringComparison.OrdinalIgnoreCase));
    }

    public static IReadOnlyCollection<MangaUpdater.Shared.Models.GenericChapterCandidate> DeduplicateCandidates(
        IEnumerable<MangaUpdater.Shared.Models.GenericChapterCandidate> candidates)
    {
        return candidates
            .GroupBy(candidate => candidate.Number)
            .Select(group => group.OrderByDescending(candidate => candidate.Date).First())
            .OrderBy(candidate => candidate.Number, StringComparer.Ordinal)
            .ToList();
    }

    private static HtmlNode? ResolveTargetNode(HtmlNode node, string? xpath)
    {
        if (string.IsNullOrWhiteSpace(xpath) || xpath == ".")
            return node;

        return node.SelectSingleNode(xpath);
    }

    private static string? ExtractInnerTextWithSpacing(HtmlNode node)
    {
        var textParts = node
            .DescendantsAndSelf()
            .Where(currentNode => currentNode.NodeType == HtmlNodeType.Text)
            .Select(currentNode => HtmlEntity.DeEntitize(currentNode.InnerText))
            .Select(text => Regex.Replace(text, "\\s+", " ").Trim())
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToArray();

        if (textParts.Length == 0)
            return null;

        return string.Join(" ", textParts);
    }

    private static string ExtractDateValue(string rawValue, IReadOnlyDictionary<string, string?> parsingProfile)
    {
        var dateRegex = GetProfileValue(parsingProfile, "ChapterDateRegex");
        if (string.IsNullOrWhiteSpace(dateRegex))
            return rawValue;

        var match = Regex.Match(rawValue, dateRegex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!match.Success)
            return rawValue;

        if (match.Groups.Count > 1 && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
            return match.Groups[1].Value.Trim();

        return match.Value.Trim();
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

        var relativePattern = GetProfileValue(parsingProfile, "RelativeDateRegex")
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
}