using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Exceptions;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.AdminServices;

public record StopAdminServiceCommand(string Name) : IRequest<string>;

public class StopAdminServiceHandler : IRequestHandler<StopAdminServiceCommand, string>
{
    private readonly IChapterTaskDispatchManager _manager;

    public StopAdminServiceHandler(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }

    public Task<string> Handle(StopAdminServiceCommand request, CancellationToken cancellationToken)
    {
        if (!_manager.TryResolveSourceId(request.Name, out var sourceId))
            throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest, "Unknown service name");

        // Stop is not supported; emulate with pause
        _manager.PauseBySource(sourceId);
        return Task.FromResult("Stop is emulated as pause; full stop not supported.");
    }
}
