using Serilog;
using Serilog.Core;

namespace MangaUpdater.Services.Logging;

public class AppLogger
{
    private readonly Logger _logger;

    public AppLogger(string connectionString, string tableName = "Logs")
    {
        _logger = new LoggerConfiguration()
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: tableName,
                needAutoCreateTable: true
            )
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.Information(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.Warning(message, args);
    }

    public void LogError(string message, Exception? ex = null, params object[] args)
    {
        if (ex != null)
            _logger.Error(ex, message, args);
        else
            _logger.Error(message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.Debug(message, args);
    }
}