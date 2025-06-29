namespace MangaUpdater.Shared.DTOs;

public record ChapterDto(int ChapterId, string Manga, string Source, string Number, DateTime Timestamp);