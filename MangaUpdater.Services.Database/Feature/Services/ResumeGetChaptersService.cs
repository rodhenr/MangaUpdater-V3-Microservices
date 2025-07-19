using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Services;

public record ResumeGetChaptersServiceCommand : IRequest;

public class ResumeGetChaptersService : IRequestHandler<ResumeGetChaptersServiceCommand>
{
    private readonly IChapterTaskDispatchManager _manager;

    public ResumeGetChaptersService(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
        
    }
    public Task Handle(ResumeGetChaptersServiceCommand request, CancellationToken cancellationToken)
    {
        _manager.Resume();
        return Task.CompletedTask;
    }
}