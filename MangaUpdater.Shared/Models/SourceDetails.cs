namespace MangaUpdater.Shared.Models;

public record SourceDetails(int SourceId, string SourceName, string QueueName, DateTime? NextExecutionDate,
	TimeSpan Delay, string State);