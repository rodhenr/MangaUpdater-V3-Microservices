using System.Reflection;
using MangaUpdater.Service.Messaging.Services;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.Chapters;
using MangaUpdater.Services.Database.Services;
using MangaUpdater.Services.Logging;
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
        services.AddHttpClient();
        
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
    
    private static void AddDatabaseServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }

    private static void AddRabbitMqServices(IServiceCollection services, IConfiguration configuration)
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
    }

    private static void AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<ChapterSaverService>();
        services.AddHostedService<ChapterTaskDispatcherService>();
    }

    private static void AddSerilogServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAppLogger, AppLogger>(_ =>
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection");

            return defaultConnection is null ? throw new Exception("Invalid connection string") : new AppLogger(defaultConnection);
        });
    }
}