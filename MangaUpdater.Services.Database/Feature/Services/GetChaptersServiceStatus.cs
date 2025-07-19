using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.Services;

public record GetGetChaptersServiceStatusQuery(SourcesEnum Source) : IRequest<SourceDetails>;

public class GetChaptersServiceStatus : IRequestHandler<GetGetChaptersServiceStatusQuery, SourceDetails>
{
    private readonly IChapterTaskDispatchManager _manager;

    public GetChaptersServiceStatus(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }
    
    public Task<SourceDetails> Handle(GetGetChaptersServiceStatusQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_manager.GetExecutionDetails(request.Source));
    }
}