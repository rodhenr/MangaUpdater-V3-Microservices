namespace MangaUpdater.Shared.DTOs;

public record FetcherChapterResultDto(int MangaId, int SourceId, string Number, DateTime Date);