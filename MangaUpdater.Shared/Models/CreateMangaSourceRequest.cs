namespace MangaUpdater.Shared.Models;

public record CreateMangaSourceRequest(int MangaId, int SourceId, string Url);