using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class Source : BaseEntity
{
    [StringLength(200)]
    public required string Name { get; set; }

    [StringLength(100)]
    public string? Slug { get; set; }
    
    [StringLength(200)]
    public required string BaseUrl { get; set; }

    public bool IsEnabled { get; set; } = true;

    [StringLength(50)]
    public string EngineType { get; set; } = "HtmlXPath";

    [StringLength(50)]
    public string RequestMode { get; set; } = "HttpGet";

    public bool RequiresBrowser { get; set; }

    [StringLength(500)]
    public string? DefaultUserAgent { get; set; }

    public int? RateLimitMilliseconds { get; set; }

    [StringLength(150)]
    public string? QueueName { get; set; }

    public bool SupportsPagination { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    [InverseProperty("Source")]
    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    
    [InverseProperty("Source")]
    public virtual ICollection<MangaSource> MangaSources { get; set; } = new List<MangaSource>();

    [InverseProperty("Source")]
    public virtual ICollection<SourceApiProfile> ApiProfiles { get; set; } = new List<SourceApiProfile>();

    [InverseProperty("Source")]
    public virtual ICollection<SourceRequestProfile> RequestProfiles { get; set; } = new List<SourceRequestProfile>();

    [InverseProperty("Source")]
    public virtual ICollection<SourceScrapingProfile> ScrapingProfiles { get; set; } = new List<SourceScrapingProfile>();
}