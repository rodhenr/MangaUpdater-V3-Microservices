using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Exceptions;
using MediatR;

namespace MangaUpdater.Services.Database.Feature.AdminServices;

public record StartAdminServiceCommand(string Name) : IRequest;

public class StartAdminServiceHandler : IRequestHandler<StartAdminServiceCommand>
{
    private readonly IChapterTaskDispatchManager _manager;

    public StartAdminServiceHandler(IChapterTaskDispatchManager manager)
    {
        _manager = manager;
    }

    public Task Handle(StartAdminServiceCommand request, CancellationToken cancellationToken)
    {
        if (!_manager.TryResolveSourceId(request.Name, out var sourceId))
            throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest, "Unknown service name");

        _manager.ResumeBySource(sourceId);
        return Task.CompletedTask;
    }
}
