using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaUpdater.Services.Database.Entities;

public class Manga : BaseEntity
{
    public int? MyAnimeListId { get; set; }
    
    public int AniListId { get; set; }
    
    [StringLength(200)]
    public required string TitleRomaji { get; set; }
    
    [StringLength(200)]
    public string? TitleEnglish { get; set; }
    
    public required string CoverUrl { get; set; }

    public bool IsAutoCreated { get; set; }

    public DateTime? LastUpdate { get; set; }

    public DateTime Timestamp { get; set; }
    
    [InverseProperty("Manga")]
    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    
    [InverseProperty("Manga")]
    public virtual ICollection<MangaSource> MangaSources { get; set; } = new List<MangaSource>();
}