namespace MangaUpdater.Services.API.DTOs;

public record UserChaptersDto(int? MyAnimeListId, string? UrlMyAnimeList, int? AnilistId, string? UrlAnilist, string TitleRomaji, string TitleEnglish, decimal? SourceLastChapterNumber, decimal UserLastChapterNumber);