using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class SourceScrapingProfile : BaseEntity
{
    public int SourceId { get; set; }

    public bool IsActive { get; set; } = true;

    public int Version { get; set; } = 1;

    [StringLength(1000)]
    public string? ChapterNodesXPath { get; set; }

    [StringLength(1000)]
    public string? ChapterUrlXPath { get; set; }

    [StringLength(100)]
    public string? ChapterUrlAttribute { get; set; }

    [StringLength(1000)]
    public string? ChapterNumberXPath { get; set; }

    [StringLength(100)]
    public string? ChapterNumberAttribute { get; set; }

    [StringLength(1000)]
    public string? ChapterNumberRegex { get; set; }

    [StringLength(1000)]
    public string? ChapterDateXPath { get; set; }

    [StringLength(100)]
    public string? ChapterDateAttribute { get; set; }

    [StringLength(1000)]
    public string? ChapterDateRegex { get; set; }

    [StringLength(50)]
    public string? DateParseMode { get; set; }

    [StringLength(50)]
    public string? DateCulture { get; set; }

    [StringLength(100)]
    public string? DateFormatPrimary { get; set; }

    [StringLength(100)]
    public string? DateFormatSecondary { get; set; }

    [StringLength(1000)]
    public string? RelativeDateRegex { get; set; }

    [StringLength(200)]
    public string? IgnoreTextContains1 { get; set; }

    [StringLength(200)]
    public string? IgnoreTextContains2 { get; set; }

    [StringLength(200)]
    public string? IgnoreTextContains3 { get; set; }

    [StringLength(500)]
    public string? UrlPrefix { get; set; }

    [StringLength(50)]
    public string? UrlJoinMode { get; set; }

    [StringLength(50)]
    public string? DeduplicationKeyMode { get; set; }

    [StringLength(50)]
    public string? ChapterSortMode { get; set; }

    [StringLength(1000)]
    public string? PaginationNextPageXPath { get; set; }

    [StringLength(1000)]
    public string? PaginationUrlTemplate { get; set; }

    public int? ResultLimit { get; set; }

    public DateTime Timestamp { get; set; }

    [ForeignKey("SourceId")]
    [InverseProperty("ScrapingProfiles")]
    public virtual Source Source { get; set; } = null!;
}