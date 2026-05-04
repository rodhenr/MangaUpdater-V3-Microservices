namespace MangaUpdater.Shared.Models;

public record GenericChapterCandidate(string Number, DateTime Date, string Url, string? RawTitle = null);