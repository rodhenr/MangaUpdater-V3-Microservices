namespace MangaUpdater.Services.Fetcher.Models;

public class RabbitMQSettings
{
    public required string HostName { get; set; }
    
    public required string UserName { get; set; }
    
    public required string Password { get; set; }
}