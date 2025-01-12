namespace MangaUpdater.Shared.Models;

public record CreateMangaRequest(string CoverUrl, int MyAnimeListId, int AniListId, string TitleRomaji, string TitleEnglish);