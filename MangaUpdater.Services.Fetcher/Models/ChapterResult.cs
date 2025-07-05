namespace MangaUpdater.Services.Fetcher.Models;

public record ChapterResult(int MangaId, string MangaName, int SourceId, string Number, DateTime Date, string Url);