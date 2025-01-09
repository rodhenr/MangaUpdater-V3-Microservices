using System.Text.Json.Serialization;

namespace MangaUpdater.Services.Fetcher.Models;

public class AsuraScansDto
{
    [JsonPropertyName("chapters")] 
    public List<AsuraScansChapter> Chapters { get; set; } = [];
}

public class AsuraScansChapter
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public decimal Number { get; set; }
    
    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
}

