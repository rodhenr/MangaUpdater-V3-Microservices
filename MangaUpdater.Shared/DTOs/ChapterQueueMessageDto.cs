using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Shared.DTOs;

public record ChapterQueueMessageDto(string BaseUrlPart, string MangaUrlPart, string? AdditionalInfo, int MangaId, string MangaName, SourcesEnum Source, string LastChapterNumber);