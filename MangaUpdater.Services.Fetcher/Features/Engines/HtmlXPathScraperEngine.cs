using HtmlAgilityPack;
using MangaUpdater.Services.Fetcher.Helpers;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.Extensions;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;

namespace MangaUpdater.Services.Fetcher.Features.Engines;

public class HtmlXPathScraperEngine : IScraperEngine
{
    private readonly ISourceRequestExecutor _sourceRequestExecutor;
    private readonly IAppLogger _appLogger;

    public HtmlXPathScraperEngine(ISourceRequestExecutor sourceRequestExecutor, IAppLogger appLogger)
    {
        _sourceRequestExecutor = sourceRequestExecutor;
        _appLogger = appLogger;
    }

    public string EngineType => "HtmlXPath";

    public bool CanHandle(SourceRuntimeDefinition sourceRuntimeDefinition)
    {
        return string.Equals(sourceRuntimeDefinition.EngineType, EngineType, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<SourceParsingResult> FetchAsync(SourceRuntimeDefinition sourceRuntimeDefinition,
        GenericScraperContext context, CancellationToken cancellationToken = default)
    {
        var diagnostics = new List<string>();
        var warnings = new List<string>();

        try
        {
            var chapterNodesXPath = HtmlXPathExtractionHelpers.GetRequiredProfileValue(
                sourceRuntimeDefinition.ParsingProfile,
                "ChapterNodesXPath");

            if (string.IsNullOrWhiteSpace(chapterNodesXPath))
            {
                var message =
                    $"HtmlXPath source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}' is missing required parsing key 'ChapterNodesXPath'.";
                _appLogger.LogError("Fetch", message);
                _sourceRequestExecutor.RegisterConfigurationFailure(sourceRuntimeDefinition, message);
                diagnostics.Add(message);
                return new SourceParsingResult(sourceRuntimeDefinition.SourceId, Array.Empty<GenericChapterCandidate>(), warnings,
                    diagnostics);
            }

            var requestUrl = HtmlXPathExtractionHelpers.BuildRequestUrl(sourceRuntimeDefinition, context);
            var html = await _sourceRequestExecutor.GetStringAsync(sourceRuntimeDefinition, requestUrl, cancellationToken);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var chapterNodes = htmlDocument.DocumentNode.SelectNodes(chapterNodesXPath)?.ToList() ?? [];
            _appLogger.LogInformation(
                "Fetch",
                "HtmlXPath source '{0}' ({1}) engine '{2}' profile '{3}' matched {4} chapter nodes before filtering. Manga '{5}'. Url: {6}",
                sourceRuntimeDefinition.SourceName,
                sourceRuntimeDefinition.SourceId,
                sourceRuntimeDefinition.EngineType,
                HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown",
                chapterNodes.Count,
                context.MangaName,
                requestUrl);

            if (chapterNodes.Count == 0)
            {
                var warning =
                    $"No chapter nodes were found for xpath '{chapterNodesXPath}' on source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}'.";
                warnings.Add(warning);
                _appLogger.LogWarning("Fetch", warning);
                _sourceRequestExecutor.RegisterConfigurationFailure(sourceRuntimeDefinition, warning);
                return new SourceParsingResult(sourceRuntimeDefinition.SourceId, Array.Empty<GenericChapterCandidate>(), warnings,
                    diagnostics);
            }

            var lastChapterDecimal = context.LastChapterNumber.GetNumericPart();
            var candidates = new List<GenericChapterCandidate>();
            var extractedNumbers = 0;

            foreach (var chapterNode in chapterNodes)
            {
                if (HtmlXPathExtractionHelpers.ShouldIgnoreNode(chapterNode, sourceRuntimeDefinition.ParsingProfile))
                    continue;

                var urlValue = HtmlXPathExtractionHelpers.ExtractNodeValue(
                    chapterNode,
                    HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ChapterUrlXPath"),
                    HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ChapterUrlAttribute"));

                if (string.IsNullOrWhiteSpace(urlValue))
                    continue;

                var chapterNumberSource = HtmlXPathExtractionHelpers.ExtractNodeValue(
                                              chapterNode,
                                              HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile,
                                                  "ChapterNumberXPath"),
                                              HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile,
                                                  "ChapterNumberAttribute"))
                                          ?? urlValue;

                var chapterNumber = HtmlXPathExtractionHelpers.NormalizeChapterNumber(
                    chapterNumberSource,
                    HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile,
                        "ChapterNumberRegex"));

                if (string.IsNullOrWhiteSpace(chapterNumber))
                    continue;

                extractedNumbers++;

                if (chapterNumber.GetNumericPart() <= lastChapterDecimal)
                    continue;

                var dateValue = HtmlXPathExtractionHelpers.ExtractNodeValue(
                    chapterNode,
                    HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ChapterDateXPath"),
                    HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile,
                        "ChapterDateAttribute"));

                var parsedDate = HtmlXPathExtractionHelpers.ParseDate(
                    dateValue,
                    sourceRuntimeDefinition.ParsingProfile,
                    warnings);

                var normalizedUrl = HtmlXPathExtractionHelpers.NormalizeUrl(
                    urlValue,
                    context.SourceBaseUrl,
                    sourceRuntimeDefinition.ParsingProfile);

                candidates.Add(new GenericChapterCandidate(
                    chapterNumber,
                    parsedDate,
                    normalizedUrl,
                    chapterNode.InnerText.Trim()));
            }

            if (extractedNumbers == 0)
            {
                var message =
                    $"HtmlXPath source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}' did not extract any chapter number from {chapterNodes.Count} candidate nodes for manga '{context.MangaName}'.";
                _appLogger.LogError("Fetch", message);
                _sourceRequestExecutor.RegisterConfigurationFailure(sourceRuntimeDefinition, message);
                diagnostics.Add(message);
                return new SourceParsingResult(sourceRuntimeDefinition.SourceId, Array.Empty<GenericChapterCandidate>(), warnings,
                    diagnostics);
            }

            var deduplicatedCandidates = HtmlXPathExtractionHelpers.DeduplicateCandidates(candidates);
            _sourceRequestExecutor.RegisterSuccess(sourceRuntimeDefinition);

            return new SourceParsingResult(sourceRuntimeDefinition.SourceId, deduplicatedCandidates, warnings, diagnostics);
        }
        catch (Exception ex)
        {
            _sourceRequestExecutor.RegisterConfigurationFailure(sourceRuntimeDefinition, ex.Message);
            _appLogger.LogError(
                "Fetch",
                $"HtmlXPath engine failed for source '{sourceRuntimeDefinition.SourceName}' ({sourceRuntimeDefinition.SourceId}) engine '{sourceRuntimeDefinition.EngineType}' profile '{HtmlXPathExtractionHelpers.GetProfileValue(sourceRuntimeDefinition.ParsingProfile, "ProfileVersion") ?? "unknown"}' and manga '{context.MangaName}'.",
                ex);
            diagnostics.Add(ex.Message);
            return new SourceParsingResult(sourceRuntimeDefinition.SourceId, Array.Empty<GenericChapterCandidate>(), warnings,
                diagnostics);
        }
    }
}