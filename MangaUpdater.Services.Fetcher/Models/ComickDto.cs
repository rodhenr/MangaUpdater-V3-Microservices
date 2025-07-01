using System.Text.Json.Serialization;

namespace MangaUpdater.Services.Fetcher.Models;

public class ComickDto
{
    [JsonPropertyName("chapters")]
    public List<ComickChapter> Chapters { get; set; }
    
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    [JsonPropertyName("checkVol2Chap1")]
    public bool CheckVol2Chap1 { get; set; }
    
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

public class ComickChapter
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("chap")]
    public string Chap { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("vol")]
    public string Vol { get; set; }

    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("up_count")]
    public int UpCount { get; set; }

    [JsonPropertyName("down_count")]
    public int DownCount { get; set; }

    [JsonPropertyName("is_the_last_chapter")]
    public bool IsTheLastChapter { get; set; }

    [JsonPropertyName("publish_at")]
    public DateTime? PublishAt { get; set; }

    [JsonPropertyName("group_name")]
    public List<string> GroupName { get; set; }

    [JsonPropertyName("hid")]
    public string Hid { get; set; }

    [JsonPropertyName("identities")]
    public object Identities { get; set; }

    [JsonPropertyName("md_chapters_groups")]
    public List<MdChaptersGroup> MdChaptersGroups { get; set; }
}


public class MdChaptersGroup
{
    [JsonPropertyName("mdGroups")]
    public MdGroup MdGroups { get; set; }
}

public class MdGroup
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("slug")]
    public string Slug { get; set; }
}