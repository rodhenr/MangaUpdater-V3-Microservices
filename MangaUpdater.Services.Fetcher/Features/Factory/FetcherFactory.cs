using MangaUpdater.Services.Fetcher.Features.Apis;
using MangaUpdater.Services.Fetcher.Features.Scrapers;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Services.Fetcher.Features.Factory;

[RegisterSingleton]
public class FetcherFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FetcherFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IFetcher GetChapterFetcher(SourcesEnum source)
    {
        return source switch
        {
            SourcesEnum.Mangadex => _serviceProvider.GetRequiredService<IFetcher>(),
            SourcesEnum.AsuraScans => _serviceProvider.GetRequiredService<AsuraScansScrapper>(),
            _ => throw new Exception("No valid source found.")
        };
    }
}