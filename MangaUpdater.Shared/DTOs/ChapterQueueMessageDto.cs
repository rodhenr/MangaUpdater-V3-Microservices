namespace MangaUpdater.Shared.DTOs;

public record ChapterQueueMessageDto(
	int SourceId,
	string? SourceSlug,
	string SourceBaseUrl,
	string MangaPath,
	string? AdditionalInfo,
	int MangaId,
	string MangaName,
	string LastChapterNumber);