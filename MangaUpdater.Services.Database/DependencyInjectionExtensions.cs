using System.Reflection;
using MangaUpdater.Service.Messaging.Services;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.Chapters;
using MangaUpdater.Services.Database.Services;
using MangaUpdater.Services.Logging;
using MangaUpdater.Shared.Enums;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MangaUpdater.Services.Database;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IChapterTaskDispatchManager, ChapterTaskDispatchManager>();
        services.AddScoped<ISaveChapters, SaveChapters>();
        
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(executingAssembly);
        });
        
        AddDatabaseServices(services, configuration);
        AddRabbitMqServices(services, configuration);
        AddBackgroundServices(services);
        AddSerilogServices(services, configuration);
        
        return services;
    }
    
    private static IServiceCollection AddDatabaseServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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
        services.AddHostedService<ChapterSaverService>();
        foreach (var source in Enum.GetValues<SourcesEnum>())
        {
            services.AddSingleton<IHostedService>(provider =>
                ActivatorUtilities.CreateInstance<ChapterTaskDispatcherService>(
                    provider,
                    source));
        }

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