namespace MangaUpdater.Shared.DTOs;

public record AnilistUserMangaCollectionDto(
    int IdAnilist, 
    int IdMyAnimeList, 
    string Status, 
    int AverageScore, 
    string CountryOfOrigin, 
    string Title, 
    int UserLastChapterRead, 
    string UrlMyAnimeList, 
    string UrlAnilist, 
    string CoverImage, 
    List<string> Genres
);