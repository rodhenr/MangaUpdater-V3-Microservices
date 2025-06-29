using System.Net;

namespace MangaUpdater.Shared.Exceptions;

public class HttpResponseException : Exception
{
    public int StatusCode { get; }

    public override string Message { get; }

    public HttpResponseException(HttpStatusCode statusCode, string message)
    {
        StatusCode = (int)statusCode;
        Message = message;
    }
}