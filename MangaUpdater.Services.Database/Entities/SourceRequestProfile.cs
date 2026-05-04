using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class SourceRequestProfile : BaseEntity
{
    public int SourceId { get; set; }

    public bool IsActive { get; set; } = true;

    public int Version { get; set; } = 1;

    [StringLength(10)]
    public string Method { get; set; } = "GET";

    [StringLength(1000)]
    public string? UrlTemplate { get; set; }

    public string? HeadersJson { get; set; }

    public int? TimeoutSeconds { get; set; }

    public bool UseCookies { get; set; }

    [StringLength(50)]
    public string? AcceptLanguage { get; set; }

    [StringLength(500)]
    public string? Referrer { get; set; }

    public DateTime Timestamp { get; set; }

    [ForeignKey("SourceId")]
    [InverseProperty("RequestProfiles")]
    public virtual Source Source { get; set; } = null!;
}