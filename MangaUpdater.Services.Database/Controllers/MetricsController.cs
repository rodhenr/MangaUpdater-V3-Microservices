using MangaUpdater.Services.Database.Feature.Metrics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MangaUpdater.Services.Database.Controllers;

public class MetricsController : BaseController
{
    private readonly ISender _sender;

    public MetricsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<GetMetricsResponse> GetMetrics()
    {
        return await _sender.Send(new GetMetricsQuery());
    }
}