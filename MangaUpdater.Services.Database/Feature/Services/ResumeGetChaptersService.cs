using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Services;

public record ResumeGetChaptersServiceCommand(int SourceId) : IRequest;

public class ResumeGetChaptersService : IRequestHandler<ResumeGetChaptersServiceCommand>
{
    private readonly IChapterTaskDispatchManager _manager;

    public ResumeGetChaptersService(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
        
    }
    public Task Handle(ResumeGetChaptersServiceCommand request, CancellationToken cancellationToken)
    {
        _manager.ResumeBySource(request.SourceId);
        return Task.CompletedTask;
    }
}