using MangaUpdater.Shared.Interfaces;
using NpgsqlTypes;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;

namespace MangaUpdater.Services.Logging;

public class AppLogger : IAppLogger
{
    private readonly Logger _logger;

    public AppLogger(string connectionString, string tableName = "LogEvents")
    {
        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
        
        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "\"Message\"", new RenderedMessageColumnWriter() },
            { "\"Level\"", new LevelColumnWriter() },
            { "\"Timestamp\"", new TimestampColumnWriter() },
            { "\"Exception\"", new ExceptionColumnWriter() },
            { "\"Module\"", new SinglePropertyColumnWriter("Module") },
        };
        
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug() 
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "\"LogEvents\"",
                needAutoCreateTable: true,
                columnOptions: columnWriters
            )
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    public void LogInformation(string module, string message, params object[] args)
    {
        _logger.ForContext("Module", module).Information(message, args);
    }

    public void LogWarning(string module, string message, params object[] args)
    {
        _logger.ForContext("Module", module).Warning(message, args);
    }

    public void LogError(string module, string message, Exception? ex = null, params object[] args)
    {
        if (ex != null)
            _logger.ForContext("Module", module).Error(ex, message, args);
        else
            _logger.ForContext("Module", module).Error(message, args);
    }

    public void LogDebug(string module, string message, params object[] args)
    {
        _logger.ForContext("Module", module).Debug(message, args);
    }
}