using MangaUpdater.Shared.Enums;
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
        if (!TryResolveSourceFromName(request.Name, out var source, out var message))
            throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest, message);

        _manager.ResumeBySource(source);
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
