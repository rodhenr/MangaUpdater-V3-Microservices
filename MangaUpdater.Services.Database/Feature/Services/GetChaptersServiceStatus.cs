using MangaUpdater.Shared.Interfaces;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Services;

public record GetGetChaptersServiceStatusQuery : IRequest<GetChaptersServiceStatusDto>;
public record GetChaptersServiceStatusDto(DateTime? NextExecutionDate, TimeSpan Delay , string State);

public class GetChaptersServiceStatus : IRequestHandler<GetGetChaptersServiceStatusQuery, GetChaptersServiceStatusDto>
{
    private readonly IChapterTaskDispatchManager _manager;

    public GetChaptersServiceStatus(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }
    
    public Task<GetChaptersServiceStatusDto> Handle(GetGetChaptersServiceStatusQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetChaptersServiceStatusDto(_manager.NextExecutionUtc, _manager.Delay, _manager.State.ToString()));
    }
}