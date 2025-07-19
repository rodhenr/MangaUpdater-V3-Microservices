using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Fetcher.Features.Services;

public record GetQueueStatesQuery : IRequest<Dictionary<string, ServicesStateEnum>>;
    
public class GetQueueStates : IRequestHandler<GetQueueStatesQuery, Dictionary<string, ServicesStateEnum>>
{
    private readonly IFetchChaptersManager _manager;
    public GetQueueStates(IFetchChaptersManager manager)
    {
        _manager = manager;
    }
    
    public Task<Dictionary<string, ServicesStateEnum>> Handle(GetQueueStatesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_manager.QueueStates);
    }
}