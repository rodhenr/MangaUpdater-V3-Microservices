using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;
using System.Net;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record CreateSourceCommand(CreateSourceRequest Source) : IRequest;

public class CreateSourceHandler : IRequestHandler<CreateSourceCommand>
{
    private readonly AppDbContext _context;
    public CreateSourceHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateSourceCommand request, CancellationToken cancellationToken)
    {
        var baseUrl = request.Source.ResolveBaseUrl().Trim();
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new HttpResponseException(HttpStatusCode.BadRequest, "BaseUrl is required");

        var source = new Source
        {
            Name = request.Source.Name,
            Slug = NormalizeValue(request.Source.Slug),
            BaseUrl = baseUrl,
            IsEnabled = request.Source.IsEnabled,
            EngineType = NormalizeRequiredValue(request.Source.EngineType, "EngineType"),
            RequestMode = NormalizeRequiredValue(request.Source.RequestMode, "RequestMode"),
            RequiresBrowser = request.Source.RequiresBrowser,
            DefaultUserAgent = NormalizeValue(request.Source.DefaultUserAgent),
            RateLimitMilliseconds = request.Source.RateLimitMilliseconds,
            QueueName = NormalizeValue(request.Source.QueueName),
            SupportsPagination = request.Source.SupportsPagination,
            Timestamp = DateTime.UtcNow
        };

        _context.Sources.Add(source);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string? NormalizeValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeRequiredValue(string? value, string fieldName)
    {
        var normalized = NormalizeValue(value);
        return !string.IsNullOrWhiteSpace(normalized) 
            ? normalized 
            : throw new HttpResponseException(HttpStatusCode.BadRequest, $"{fieldName} is required");
    }
}