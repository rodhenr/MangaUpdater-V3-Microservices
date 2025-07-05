namespace MangaUpdater.Shared.DTOs;

public record MangaSourceDto(int Id, int MangaId, string MangaName, int SourceId, string SourceName, string Url, string? AdditionalInfo);