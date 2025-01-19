namespace MangaUpdater.Shared.DTOs;

public record MangaChaptersDto(string CoverUrl, int MyAnimeListId, int AniListId, string TitleRomaji, string TitleEnglish, List<ChaptersDto> Chapters);

public record ChaptersDto(int SourceId, string Number, DateTime Date, string Url);