using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Fetcher.Features.Services;

public record ResumeQueueCommand(string QueueName) : IRequest;

public class ResumeQueue : IRequestHandler<ResumeQueueCommand>
{
    private readonly IFetchChaptersManager _manager;
    
    public ResumeQueue(IFetchChaptersManager manager)
    {
        _manager = manager;
    }
    
    public Task Handle(ResumeQueueCommand request, CancellationToken cancellationToken)
    {
        _manager.ResumeQueue(request.QueueName);
        return Task.CompletedTask;
    }
}