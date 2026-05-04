using System.Globalization;
using System.Text.Json;
using MangaUpdater.Services.Fetcher.Helpers;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.Extensions;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Features.Engines;

public class JsonApiScraperEngine : IScraperEngine
{
    private readonly ISourceRequestExecutor _sourceRequestExecutor;
    private readonly IAppLogger _appLogger;

    public JsonApiScraperEngine(ISourceRequestExecutor sourceRequestExecutor, IAppLogger appLogger)
    {
        _sourceRequestExecutor = sourceRequestExecutor;
        _appLogger = appLogger;
    }

    public string EngineType => "JsonApi";

    public bool CanHandle(SourceRuntimeDefinition sourceRuntimeDefinition)
    {
        return string.Equals(sourceRuntimeDefinition.EngineType, EngineType, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<SourceParsingResult> FetchAsync(SourceRuntimeDefinition sourceRuntimeDefinition,
        GenericScraperContext context, CancellationToken cancellationToken = default)
    {
        var warnings = new List<string>();
        var diagnostics = new List<string>();
        var candidates = new List<GenericChapterCandidate>();

        try
        {
            var dataRootPath = JsonApiExtractionHelpers.GetRequiredProfileValue(sourceRuntimeDefinition.ParsingProfile, "DataRootPath");
            var chapterNumberPath = JsonApiExtractionHelpers.GetRequiredProfileValue(sourceRuntimeDefinition.ParsingProfile, "ChapterNumberPath");
            var chapterDatePath = JsonApiExtractionHelpers.GetRequiredProfileValue(sourceRuntimeDefinition.ParsingProfile, "ChapterDatePath");

            if (string.IsNullOrWhiteSpace(dataRootPath) || string.IsNullOrWhiteSpace(chapterNumberPath) ||
                string.IsNullOrWhiteSpace(chapterDatePath))
            {
                var message =
                    $"JsonApi source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{JsonApiExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}' is missing required API parsing metadata.";
                _appLogger.LogError("Fetch", message);
                _sourceRequestExecutor.RegisterConfigurationFailure(sourceRuntimeDefinition, message);
                diagnostics.Add(message);
                return new SourceParsingResult(sourceRuntimeDefinition.SourceId, Array.Empty<GenericChapterCandidate>(), warnings,
                    diagnostics);
            }

            var paginationMode = JsonApiExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "PaginationMode") ?? "Offset";
            var pageIndex = 1;
            var offset = 0;
            string? nextPageToken = null;
            var lastChapterDecimal = context.LastChapterNumber.GetNumericPart();
            var totalItemsSeen = 0;

            while (true)
            {
                var requestUrl = JsonApiExtractionHelpers.BuildRequestUrl(sourceRuntimeDefinition, context, offset, pageIndex, nextPageToken);
                var json = await _sourceRequestExecutor.GetStringAsync(sourceRuntimeDefinition, requestUrl, cancellationToken);
                using var jsonDocument = JsonDocument.Parse(json);

                var items = JsonApiExtractionHelpers.ExtractArray(jsonDocument.RootElement, dataRootPath);
                _appLogger.LogInformation(
                    "Fetch",
                    "JsonApi source '{0}' ({1}) engine '{2}' profile '{3}' fetched {4} items using pagination mode '{5}'. Manga '{6}'. Url: {7}",
                    sourceRuntimeDefinition.SourceName,
                    sourceRuntimeDefinition.SourceId,
                    sourceRuntimeDefinition.EngineType,
                    JsonApiExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown",
                    items.Count,
                    paginationMode,
                    context.MangaName,
                    requestUrl);

                if (items.Count == 0)
                    break;

                totalItemsSeen += items.Count;

                foreach (var item in items)
                {
                    var chapterNumber = JsonApiExtractionHelpers.ExtractString(item, chapterNumberPath);
                    if (string.IsNullOrWhiteSpace(chapterNumber))
                        continue;

                    if (chapterNumber.GetNumericPart() <= lastChapterDecimal)
                        continue;

                    var dateValue = JsonApiExtractionHelpers.ExtractString(item, chapterDatePath);
                    var parsedDate = JsonApiExtractionHelpers.ParseDate(dateValue, warnings);
                    var urlValue = JsonApiExtractionHelpers.ExtractString(item,
                        JsonApiExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ChapterUrlPath"));
                    var normalizedUrl = JsonApiExtractionHelpers.ResolveResultUrl(
                        urlValue,
                        sourceRuntimeDefinition.ParsingProfile,
                        context);

                    candidates.Add(new GenericChapterCandidate(chapterNumber, parsedDate, normalizedUrl));
                }

                if (!JsonApiExtractionHelpers.TryAdvancePagination(
                        paginationMode,
                        sourceRuntimeDefinition.ParsingProfile,
                        jsonDocument.RootElement,
                        items.Count,
                        ref offset,
                        ref pageIndex,
                        ref nextPageToken))
                    break;
            }

            if (totalItemsSeen == 0)
            {
                var warning =
                    $"No items were returned for data root path '{dataRootPath}' on source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{JsonApiExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}'.";
                warnings.Add(warning);
                _appLogger.LogWarning("Fetch", warning);
                _sourceRequestExecutor.RegisterConfigurationFailure(sourceRuntimeDefinition, warning);
            }

            var deduplicatedCandidates = HtmlXPathExtractionHelpers.DeduplicateCandidates(candidates);
            if (deduplicatedCandidates.Count == 0 && totalItemsSeen > 0)
            {
                var diagnostic =
                    $"JsonApi source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{JsonApiExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}' returned {totalItemsSeen} items but no new chapters were extracted for manga '{context.MangaName}'.";
                diagnostics.Add(diagnostic);
                _appLogger.LogWarning("Fetch", diagnostic);
            }
            _sourceRequestExecutor.RegisterSuccess(sourceRuntimeDefinition);
            return new SourceParsingResult(sourceRuntimeDefinition.SourceId, deduplicatedCandidates, warnings, diagnostics);
        }
        catch (Exception ex)
        {
            _sourceRequestExecutor.RegisterConfigurationFailure(sourceRuntimeDefinition, ex.Message);
            _appLogger.LogError(
                "Fetch",
                $"JsonApi engine failed for source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{JsonApiExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}' and manga '{context.MangaName}'.",
                ex);
            diagnostics.Add(ex.Message);
            return new SourceParsingResult(sourceRuntimeDefinition.SourceId, Array.Empty<GenericChapterCandidate>(), warnings,
                diagnostics);
        }
    }
}