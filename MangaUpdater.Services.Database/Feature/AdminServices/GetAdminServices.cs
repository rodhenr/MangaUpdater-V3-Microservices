using MangaUpdater.Shared.Interfaces;
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

        foreach (var details in _manager.GetAllExecutionDetails())
        {
            var hasMessages = await _rabbit.HasMessagesInQueueAsync(details.QueueName, cancellationToken);
            results.Add(new AdminServiceStatusDto(
                Name: $"ChapterDispatcher.{details.SourceId}",
                Status: details.State,
                QueueLength: hasMessages ? 1 : 0,
                LastRunAt: null,
                LastError: null
            ));
        }

        return results;
    }
}
