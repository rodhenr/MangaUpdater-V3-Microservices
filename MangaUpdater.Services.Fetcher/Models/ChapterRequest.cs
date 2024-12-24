using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Services.Fetcher.Models;

public record ChapterRequest(string FullUrl, int MangaId, SourcesEnum Source, float LastChapterNumber);