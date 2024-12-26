namespace MangaUpdater.Shared.DTOs;

public record AnilistUserMangaCollectionDto(int IdAnilist, int IdMyAnimeList, string Title, int UserLastChapterRead, string UrlMyAnimeList, string UrlAnilist, string CoverImage);