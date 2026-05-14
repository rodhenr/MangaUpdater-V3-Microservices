namespace MangaUpdater.Shared.Models;

public record UpdateMangaRequest(int? MyAnimeListId, int AniListId, string TitleRomaji, string? TitleEnglish, string CoverUrl, bool IsAutoCreated = false, DateTime? LastUpdate = null);