namespace MangaUpdater.Services.Database.Entities;

public class User: BaseEntity
{
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "user";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
