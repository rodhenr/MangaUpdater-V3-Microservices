using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class MangaSource : BaseEntity
{
    public int MangaId { get; set; }
    
    public int SourceId { get; set; }
    
    [StringLength(100)]
    public required string Url { get; set; }
    
    [StringLength(50)]
    public string? AditionalInfo { get; set; }

    public DateTime Timestamp { get; set; }
    
    [ForeignKey("MangaId")]
    [InverseProperty("MangaSources")]
    public virtual Manga Manga { get; set; } = null!;

    [ForeignKey("SourceId")]
    [InverseProperty("MangaSources")]
    public virtual Source Source { get; set; } = null!;
}