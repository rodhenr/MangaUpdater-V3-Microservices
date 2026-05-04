using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Services;

public record TriggerGetChapterServiceCommand(int SourceId) : IRequest;

public class TriggerGetChapterService : IRequestHandler<TriggerGetChapterServiceCommand>
{
    private readonly IChapterTaskDispatchManager _manager;

    public TriggerGetChapterService(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }
    
    public Task Handle(TriggerGetChapterServiceCommand request, CancellationToken cancellationToken)
    {
        _manager.TriggerNowBySource(request.SourceId);
        return Unit.Task;
    }
}