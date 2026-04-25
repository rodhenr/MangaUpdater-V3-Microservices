using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.AdminServices;

public record AdminServiceStatusDto(string Name, string Status, int QueueLength, DateTime? LastRunAt, string? LastError);

public record GetAdminServicesQuery() : IRequest<List<AdminServiceStatusDto>>;

public class GetAdminServicesHandler : IRequestHandler<GetAdminServicesQuery, List<AdminServiceStatusDto>>
{
    private readonly IChapterTaskDispatchManager _manager;
    private readonly IRabbitMqClient _rabbit;

    public GetAdminServicesHandler(IChapterTaskDispatchManager manager, IRabbitMqClient rabbit)
    {
        _manager = manager;
        _rabbit = rabbit;
    }

    public async Task<List<AdminServiceStatusDto>> Handle(GetAdminServicesQuery request, CancellationToken cancellationToken)
    {
        var results = new List<AdminServiceStatusDto>();

        foreach (var s in Enum.GetValues<SourcesEnum>())
        {
            var details = _manager.GetExecutionDetails(s);
            var queueName = $"get-chapters-{s}";
            var hasMessages = await _rabbit.HasMessagesInQueueAsync(queueName, cancellationToken);
            results.Add(new AdminServiceStatusDto(
                Name: $"ChapterDispatcher.{s}",
                Status: details.State,
                QueueLength: hasMessages ? 1 : 0,
                LastRunAt: null,
                LastError: null
            ));
        }

        return results;
    }
}
