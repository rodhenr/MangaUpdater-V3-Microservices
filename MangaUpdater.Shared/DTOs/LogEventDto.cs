namespace MangaUpdater.Shared.DTOs;

public record LogEventDto
(
    DateTime Timestamp,
    string Module,
    int Level,
    string Message,
    string? Exception
);
