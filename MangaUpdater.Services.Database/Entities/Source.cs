using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class Source : BaseEntity
{
    [StringLength(200)]
    public required string Name { get; set; }
    
    [StringLength(200)]
    public required string BaseUrl { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    [InverseProperty("Source")]
    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    
    [InverseProperty("Source")]
    public virtual ICollection<MangaSource> MangaSources { get; set; } = new List<MangaSource>();
}