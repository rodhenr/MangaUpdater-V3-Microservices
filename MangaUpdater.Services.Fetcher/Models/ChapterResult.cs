namespace MangaUpdater.Services.Fetcher.Models;

public record ChapterResult(int MangaId, int SourceId, string Number, DateTime Date);