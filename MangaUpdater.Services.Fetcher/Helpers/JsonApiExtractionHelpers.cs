using System.Globalization;
using System.Text.Json;

namespace MangaUpdater.Services.Fetcher.Helpers;

internal static class JsonApiExtractionHelpers
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
        MangaUpdater.Shared.Models.GenericScraperContext context, int offset, int page, string? nextPageToken)
    {
        var template = runtimeDefinition.RequestProfile.GetValueOrDefault("UrlTemplate");
        if (string.IsNullOrWhiteSpace(template))
            return $"{context.SourceBaseUrl}{context.MangaPath}";

        return template
            .Replace("{BaseUrl}", context.SourceBaseUrl, StringComparison.Ordinal)
            .Replace("{SourceBaseUrl}", context.SourceBaseUrl, StringComparison.Ordinal)
            .Replace("{MangaPath}", context.MangaPath, StringComparison.Ordinal)
            .Replace("{MangaUrlPart}", context.MangaPath, StringComparison.Ordinal)
            .Replace("{AdditionalInfo}", context.AdditionalInfo ?? string.Empty, StringComparison.Ordinal)
            .Replace("{Offset}", offset.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{Page}", page.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{NextPageToken}", nextPageToken ?? string.Empty, StringComparison.Ordinal);
    }

    public static List<JsonElement> ExtractArray(JsonElement rootElement, string path)
    {
        var target = TraversePath(rootElement, path);
        if (target is null || target.Value.ValueKind != JsonValueKind.Array)
            return [];

        return target.Value.EnumerateArray().ToList();
    }

    public static string? ExtractString(JsonElement element, string? path)
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

    public static DateTime ParseDate(string? rawValue, ICollection<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return DateTime.UtcNow;

        if (DateTime.TryParse(rawValue, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsedDate))
            return parsedDate;

        warnings.Add($"Could not parse JSON API date value '{rawValue}', defaulting to current UTC time.");
        return DateTime.UtcNow;
    }

    public static string ResolveResultUrl(string? rawValue, IReadOnlyDictionary<string, string?> parsingProfile,
        MangaUpdater.Shared.Models.GenericScraperContext context)
    {
        var resultUrlTemplate = GetProfileValue(parsingProfile, "ResultUrlTemplate");
        if (!string.IsNullOrWhiteSpace(resultUrlTemplate))
        {
            return resultUrlTemplate
                .Replace("{Value}", rawValue ?? string.Empty, StringComparison.Ordinal)
                .Replace("{MangaPath}", context.MangaPath, StringComparison.Ordinal)
                .Replace("{AdditionalInfo}", context.AdditionalInfo ?? string.Empty, StringComparison.Ordinal);
        }

        if (string.IsNullOrWhiteSpace(rawValue))
            return string.Empty;

        return Uri.TryCreate(rawValue, UriKind.Absolute, out var absoluteUri)
            ? absoluteUri.ToString()
            : $"{context.SourceBaseUrl.TrimEnd('/')}/{rawValue.TrimStart('/')}";
    }

    public static bool TryAdvancePagination(string paginationMode,
        IReadOnlyDictionary<string, string?> parsingProfile,
        JsonElement rootElement,
        int lastPageItemCount,
        ref int offset,
        ref int page,
        ref string? nextPageToken)
    {
        var normalizedMode = paginationMode.Trim();
        var resultLimit = int.TryParse(GetProfileValue(parsingProfile, "ResultLimit"), out var parsedLimit)
            ? parsedLimit
            : 200;

        if (string.Equals(normalizedMode, "Offset", StringComparison.OrdinalIgnoreCase))
        {
            if (lastPageItemCount < resultLimit)
                return false;

            offset += resultLimit;
            return true;
        }

        if (string.Equals(normalizedMode, "Page", StringComparison.OrdinalIgnoreCase))
        {
            if (lastPageItemCount < resultLimit)
                return false;

            page++;
            return true;
        }

        if (!string.Equals(normalizedMode, "NextPageToken", StringComparison.OrdinalIgnoreCase))
            return false;

        var nextPagePath = GetProfileValue(parsingProfile, "NextPagePath");
        nextPageToken = ExtractString(rootElement, nextPagePath);

        return !string.IsNullOrWhiteSpace(nextPageToken);
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
}