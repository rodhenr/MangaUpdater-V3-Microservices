using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Services;

public record SetGetChaptersServiceDelayCommand(ChapterServiceSetDelayRequest Data) : IRequest;

public class SetGetChaptersServiceDelay : IRequestHandler<SetGetChaptersServiceDelayCommand>
{
    private readonly IChapterTaskDispatchManager _manager;

    public SetGetChaptersServiceDelay(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }
    
    public Task Handle(SetGetChaptersServiceDelayCommand request, CancellationToken cancellationToken)
    {
        _manager.Delay = TimeSpan.FromMinutes(request.Data.Minutes);
        return Task.CompletedTask;
    }
}