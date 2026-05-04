using System.Net;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Entities;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Feature.Sources;

public record UpdateSourceCommand(int SourceId, UpdateSourceRequest Source) : IRequest;

public class UpdateSourceHandler : IRequestHandler<UpdateSourceCommand>
{
    private readonly AppDbContext _context;
    public UpdateSourceHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSourceCommand request, CancellationToken cancellationToken)
    {
        var source = await _context.Sources
            .Where(x => x.Id == request.SourceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (source is null) throw new HttpResponseException(HttpStatusCode.BadRequest, "Source not found");

        var baseUrl = request.Source.ResolveBaseUrl().Trim();
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new HttpResponseException(HttpStatusCode.BadRequest, "BaseUrl is required");

        source.Name = request.Source.Name;
        source.Slug = NormalizeValue(request.Source.Slug);
        source.BaseUrl = baseUrl;
        source.IsEnabled = request.Source.IsEnabled;
        source.EngineType = NormalizeRequiredValue(request.Source.EngineType, "EngineType");
        source.RequestMode = NormalizeRequiredValue(request.Source.RequestMode, "RequestMode");
        source.RequiresBrowser = request.Source.RequiresBrowser;
        source.DefaultUserAgent = NormalizeValue(request.Source.DefaultUserAgent);
        source.RateLimitMilliseconds = request.Source.RateLimitMilliseconds;
        source.QueueName = NormalizeValue(request.Source.QueueName);
        source.SupportsPagination = request.Source.SupportsPagination;
        source.Timestamp = DateTime.UtcNow;

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
