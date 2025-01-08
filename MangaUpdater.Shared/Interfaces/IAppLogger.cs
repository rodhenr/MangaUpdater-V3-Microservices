namespace MangaUpdater.Shared.Interfaces;

public interface IAppLogger
{
    void LogInformation(string module, string message, params object[] args);
    void LogWarning(string module, string message, params object[] args);
    void LogError(string module, string message, Exception? ex = null, params object[] args);
    void LogDebug(string module, string message, params object[] args);
}