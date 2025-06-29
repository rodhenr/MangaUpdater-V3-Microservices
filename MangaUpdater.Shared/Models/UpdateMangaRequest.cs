namespace MangaUpdater.Shared.Models;

public record UpdateMangaRequest(int MyAnimeListId, int AniListId, string TitleRomaji, string TitleEnglish, string CoverUrl, DateTime Timestamp);