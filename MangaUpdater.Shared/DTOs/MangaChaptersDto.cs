namespace MangaUpdater.Shared.DTOs;

public record MangaChaptersDto(int MyAnimeListId, int AniListId, string TitleRomaji, string TitleEnglish, List<ChaptersDto> Chapters);

public record ChaptersDto(int SourceId, decimal Number, DateTime Date);