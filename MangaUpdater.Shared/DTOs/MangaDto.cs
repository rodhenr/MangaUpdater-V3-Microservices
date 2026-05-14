namespace MangaUpdater.Shared.DTOs;

public record MangaDto(int Id, int? MyAnimeListId, int AniListId, string TitleRomaji, string? TitleEnglish, string CoverUrl, bool IsAutoCreated, DateTime? LastUpdate, DateTime CreatedAt);
