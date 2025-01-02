namespace MangaUpdater.Shared.Models;

public class RabbitMqSettings
{
    public required string Hostname { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public int Port { get; set; }
}