namespace MangaUpdater.Shared.Models;

public class RabbitMqSettings
{
    public required string HostName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public int Port { get; set; }
}