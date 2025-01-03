namespace MangaUpdater.Shared.Interfaces;

public interface IAppLogger
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, Exception? ex = null, params object[] args);
    void LogDebug(string message, params object[] args);
}