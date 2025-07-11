using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class Chapter : BaseEntity
{
    public int MangaId { get; set; }

    public int SourceId { get; set; }

    [MaxLength(10)]
    public required string OriginalNumber { get; set; }
    
    public int NumberMajor { get; set; }
    
    public int NumberMinor { get; set; }
    
    [MaxLength(5)]
    public string NumberSuffix { get; set; } = string.Empty;

    public DateTime Date { get; set; }
    
    [MaxLength(100)]
    public required string Url { get; set; }

    public DateTime Timestamp { get; set; }
    
    [ForeignKey("MangaId")]
    [InverseProperty("Chapters")]
    public virtual Manga Manga { get; set; } = null!;

    [ForeignKey("SourceId")]
    [InverseProperty("Chapters")]
    public virtual Source Source { get; set; } = null!;
}