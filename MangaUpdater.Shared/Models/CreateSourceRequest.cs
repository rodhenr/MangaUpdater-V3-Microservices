namespace MangaUpdater.Shared.Models;

public record CreateSourceRequest(string Name, string Url);

public record UpdateSourceRequest(string Name, string Url);