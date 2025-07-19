using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Fetcher.Features.Services;

public record PauseQueueCommand(string QueueName) : IRequest;

public class PauseQueue : IRequestHandler<PauseQueueCommand>
{
    private readonly IFetchChaptersManager _manager;
    
    public PauseQueue(IFetchChaptersManager manager)
    {
        _manager = manager;
    }
    
    public Task Handle(PauseQueueCommand request, CancellationToken cancellationToken)
    {
        _manager.PauseQueue(request.QueueName);
        return Task.CompletedTask;
    }
}