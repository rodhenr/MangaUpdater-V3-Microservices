namespace MangaUpdater.Shared.Models;

public record CreateMangaRequest(int MyAnimeListId, int AniListId, string TitleRomaji, string TitleEnglish);