namespace MangaUpdater.Services.Fetcher.Models;

public class MangaDexDto
{
    public required string Result { get; set; }
    public required string Response { get; set; }
    public required List<MangaDexResponse> Data { get; set; }
    public int? Limit { get; set; }
    public int? Offset { get; set; }
    public int? Total { get; set; }
}

public class MangaDexRelationships
{
    public required string Id { get; set; }
    public required string Type { get; set; }
}

public class MangaDexAttributes
{
    public string? Volume { get; set; }
    public required string Chapter { get; set; }
    public string? Title { get; set; }
    public required string TranslatedLanguage { get; set; }
    public string? ExternalUrl  { get; set; }
    public required string PublishAt { get; set; }
    public required string ReadableAt { get; set; }
    public required string CreatedAt { get; set; }
    public required string UpdatedAt { get; set; }
    public int Pages { get; set; }
    public int Version { get; set; }
}

public class MangaDexResponse
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public MangaDexAttributes Attributes { get; set; }
    public List<MangaDexRelationships> Relationships { get; set; }
}