using System.Text.Json;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace MangaUpdater.Shared.Middlewares;

public class HttpExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public HttpExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpResponseException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new ApiErrorResponse()
            {
                Status = ex.StatusCode,
                Error = ReasonPhrases.GetReasonPhrase(ex.StatusCode),
                Message = ex.Message,
                Method = context.Request.Method,
                Path = context.Request.Path
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new ApiErrorResponse()
            {
                Status = 500,
                Error = ReasonPhrases.GetReasonPhrase(500),
                Message = "Internal Server Error",
                Method = context.Request.Method,
                Path = context.Request.Path
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}