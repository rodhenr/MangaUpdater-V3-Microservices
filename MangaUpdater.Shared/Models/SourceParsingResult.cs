namespace MangaUpdater.Shared.Models;

public record SourceParsingResult(
    int SourceId,
    IReadOnlyCollection<GenericChapterCandidate> Chapters,
    IReadOnlyCollection<string> Warnings,
    IReadOnlyCollection<string> Diagnostics);