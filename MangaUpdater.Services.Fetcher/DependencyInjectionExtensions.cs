using MangaUpdater.Service.Messaging.Services;
using MangaUpdater.Services.Fetcher.Features.Apis;
using MangaUpdater.Services.Fetcher.Features.Scrapers;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Services;
using MangaUpdater.Services.Logging;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using Microsoft.Extensions.Options;

namespace MangaUpdater.Services.Fetcher;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<FetcherFactory>();
        services.AddTransient<IFetcher, MangadexApi>();
        services.AddTransient<AsuraScansScrapper>();
        
        AddRabbitMqServices(services, configuration);
        AddBackgroundServices(services);
        AddSerilogServices(services, configuration);
        
        return services;
    }

    private static IServiceCollection AddRabbitMqServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMqSettings"));

        services.AddTransient<IRabbitMqClient, RabbitMqClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

            return new RabbitMqClient(
                settings.Hostname,
                settings.Username,
                settings.Password,
                settings.Port
            );
        });

        return services;
    }

    private static IServiceCollection AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<GetChaptersBackgroundService>();
        
        return services;
    }

    private static IServiceCollection AddSerilogServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAppLogger, AppLogger>(sp =>
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection");

            if (defaultConnection is null) throw new Exception("Invalid connection string");

            return new AppLogger(defaultConnection);
        });

        return services;
    }
}