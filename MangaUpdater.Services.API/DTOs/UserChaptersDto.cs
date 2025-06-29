namespace MangaUpdater.Services.API.DTOs;

public record UserChaptersDto(
    int? MyAnimeListId, 
    string? UrlMyAnimeList, 
    int? AnilistId, 
    string? UrlAnilist, 
    string TitleRomaji, 
    string TitleEnglish, 
    string? SourceLastChapterNumber, 
    DateTime? SourceLastChapterDate,
    decimal UserLastChapterNumber,
    string CoverUrl,
    string Status,
    int Score,
    List<string> Genres,
    string CountryOfOrigin
);