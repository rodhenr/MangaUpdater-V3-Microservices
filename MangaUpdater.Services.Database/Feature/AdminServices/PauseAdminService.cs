using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Exceptions;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.AdminServices;

public record PauseAdminServiceCommand(string Name) : IRequest;

public class PauseAdminServiceHandler : IRequestHandler<PauseAdminServiceCommand>
{
    private readonly IChapterTaskDispatchManager _manager;

    public PauseAdminServiceHandler(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }

    public Task Handle(PauseAdminServiceCommand request, CancellationToken cancellationToken)
    {
        if (!_manager.TryResolveSourceId(request.Name, out var sourceId))
            throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest, "Unknown service name");

        _manager.PauseBySource(sourceId);
        return Task.CompletedTask;
    }
}
