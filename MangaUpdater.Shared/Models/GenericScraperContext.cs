namespace MangaUpdater.Shared.Models;

public record GenericScraperContext(
    int SourceId,
    string? SourceSlug,
    string SourceBaseUrl,
    string MangaPath,
    string? AdditionalInfo,
    int MangaId,
    string MangaName,
    string LastChapterNumber);