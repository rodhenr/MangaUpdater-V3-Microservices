using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class SourceApiProfile : BaseEntity
{
    public int SourceId { get; set; }

    public bool IsActive { get; set; } = true;

    public int Version { get; set; } = 1;

    [StringLength(1000)]
    public string? EndpointTemplate { get; set; }

    [StringLength(10)]
    public string HttpMethod { get; set; } = "GET";

    [StringLength(200)]
    public string? DataRootPath { get; set; }

    [StringLength(200)]
    public string? ChapterNumberPath { get; set; }

    [StringLength(200)]
    public string? ChapterDatePath { get; set; }

    [StringLength(200)]
    public string? ChapterUrlPath { get; set; }

    [StringLength(1000)]
    public string? ResultUrlTemplate { get; set; }

    [StringLength(50)]
    public string? PaginationMode { get; set; }

    [StringLength(50)]
    public string? OffsetParameterName { get; set; }

    [StringLength(50)]
    public string? LimitParameterName { get; set; }

    [StringLength(200)]
    public string? NextPagePath { get; set; }

    public int? ResultLimit { get; set; }

    public DateTime Timestamp { get; set; }

    [ForeignKey("SourceId")]
    [InverseProperty("ApiProfiles")]
    public virtual Source Source { get; set; } = null!;
}