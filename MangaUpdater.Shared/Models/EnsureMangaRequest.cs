namespace MangaUpdater.Shared.Models;

public record EnsureMangaRequest(
    int AniListId,
    int? MyAnimeListId,
    string TitleRomaji,
    string? TitleEnglish,
    string CoverUrl,
    bool IsAutoCreated = true,
    DateTime? LastUpdate = null
);