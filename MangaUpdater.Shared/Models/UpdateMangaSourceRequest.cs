namespace MangaUpdater.Shared.Models;

public record UpdateMangaSourceRequest(int MangaId, int SourceId, string Url, string AdditionalInfo);