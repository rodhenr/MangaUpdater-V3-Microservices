namespace MangaUpdater.Services.API.DTOs;

public record UserChaptersDto(int? MyAnimeListId, string? UrlMyAnimeList, int? AnilistId, string? UrlAnilist, string TitleRomaji, string TitleEnglish, bool HasNewChapter, decimal SourceLastChapterNumber, decimal UserLastChapterNumber);