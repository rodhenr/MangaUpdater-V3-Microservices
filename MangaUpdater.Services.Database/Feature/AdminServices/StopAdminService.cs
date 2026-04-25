using MangaUpdater.Shared.Enums;
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
        if (!TryResolveSourceFromName(request.Name, out var source, out var message))
            throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest, message);

        // Stop is not supported; emulate with pause
        _manager.PauseBySource(source);
        return Task.FromResult("Stop is emulated as pause; full stop not supported.");
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
