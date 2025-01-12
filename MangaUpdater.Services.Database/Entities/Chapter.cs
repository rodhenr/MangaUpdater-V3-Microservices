using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class Chapter : BaseEntity
{
    public int MangaId { get; set; }

    public int SourceId { get; set; }

    public decimal Number { get; set; }

    public DateTime Date { get; set; }
    
    public required string Url { get; set; }
    
    [ForeignKey("MangaId")]
    [InverseProperty("Chapters")]
    public virtual Manga Manga { get; set; } = null!;

    [ForeignKey("SourceId")]
    [InverseProperty("Chapters")]
    public virtual Source Source { get; set; } = null!;
}