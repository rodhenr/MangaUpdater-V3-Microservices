namespace MangaUpdater.Shared.DTOs;

public record FetcherChapterResultDto(int MangaId, string MangaName, int SourceId, string Number, DateTime Date, string Url);