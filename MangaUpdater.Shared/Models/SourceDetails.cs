namespace MangaUpdater.Shared.Models;

public record SourceDetails(int SourceId, DateTime? NextExecutionDate, TimeSpan Delay , string State);