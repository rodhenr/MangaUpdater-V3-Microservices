using System.Text.Json;
using MangaUpdater.Shared.DTOs;
using MediatR;

namespace MangaUpdater.Services.API.Features.Logs;

public record GetLogsQuery(int PageNumber, int PageSize) : IRequest<PagedResultDto<LogEventDto>>;

public class GetLogsHandler : IRequestHandler<GetLogsQuery, PagedResultDto<LogEventDto>>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _databaseMicroserviceUrl;

    public GetLogsHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _databaseMicroserviceUrl = configuration["Microservices:Database"] ?? throw new Exception("Database url is not configured.");
    }

    public async Task<PagedResultDto<LogEventDto>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var url = $"{_databaseMicroserviceUrl}/api/log?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<PagedResultDto<LogEventDto>>(content, _jsonOptions);

        return data ?? new PagedResultDto<LogEventDto>(new List<LogEventDto>(), 0, request.PageNumber, request.PageSize, 0);
    }
}
