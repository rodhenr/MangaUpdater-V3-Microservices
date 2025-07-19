using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Services;

public record PauseGetChaptersServiceCommand : IRequest;

public class PauseGetChaptersService: IRequestHandler<PauseGetChaptersServiceCommand>
{
    private readonly IChapterTaskDispatchManager _manager;

    public PauseGetChaptersService(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }
    
    public Task Handle(PauseGetChaptersServiceCommand request, CancellationToken cancellationToken)
    {
        _manager.Pause();
        return Task.CompletedTask;
    }
}