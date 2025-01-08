using System.ComponentModel.DataAnnotations;

namespace MangaUpdater.Services.Database.Entities;

public class LogEvent : BaseEntity
{
    public DateTime Timestamp { get; set; }
    
    [StringLength(100)]
    public required string Module { get; set; }
    
    public required int Level { get; set; }
    
    public required string Message { get; set; }
    
    public string? Exception { get; set; }
}