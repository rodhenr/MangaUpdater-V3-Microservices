using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MangaUpdater.Shared.Middlewares;

public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        string correlationId = context.Request.Headers[CorrelationIdHeader];
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers[CorrelationIdHeader] = correlationId;
        }

        context.Response.Headers[CorrelationIdHeader] = correlationId;
        context.Items[CorrelationIdHeader] = correlationId;

        using (_logger.BeginScope(new[] { new KeyValuePair<string, object>("CorrelationId", correlationId) }))
        {
            await _next(context);
        }
    }
}
