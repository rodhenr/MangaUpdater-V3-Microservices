using MangaUpdater.Services.Fetcher.Features.Apis;
using MangaUpdater.Services.Fetcher.Features.Scrapers;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Shared.Enums;

namespace MangaUpdater.Services.Fetcher.Services;

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
            SourcesEnum.VortexScans => _serviceProvider.GetRequiredService<VortexScansApi>(),
            SourcesEnum.Batoto => _serviceProvider.GetRequiredService<BatotoScrapper>(),
            SourcesEnum.SnowMachine => _serviceProvider.GetRequiredService<SnowMachineScrapper>(),
            SourcesEnum.Comick => _serviceProvider.GetRequiredService<ComickScrapper>(),
            _ => throw new Exception("No valid source found.")
        };
    }
}