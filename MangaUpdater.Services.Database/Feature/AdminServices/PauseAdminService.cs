using MangaUpdater.Shared.Enums;
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
        if (!TryResolveSourceFromName(request.Name, out var source, out var message))
            throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest, message);

        _manager.PauseBySource(source);
        return Task.CompletedTask;
    }

    private static bool TryResolveSourceFromName(string name, out SourcesEnum source, out string error)
    {
        source = default;
        error = string.Empty;
        var candidate = name.Contains('.') ? name.Split('.').Last() : name;
        if (!Enum.TryParse<SourcesEnum>(candidate, ignoreCase: true, out source))
        {
            error = "Unknown service name";
            return false;
        }
        return true;
    }
}
