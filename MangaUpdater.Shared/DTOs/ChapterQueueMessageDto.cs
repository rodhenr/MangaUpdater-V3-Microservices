using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Shared.DTOs;

public record ChapterQueueMessageDto(string FullUrl, int MangaId, SourcesEnum Source, string LastChapterNumber);