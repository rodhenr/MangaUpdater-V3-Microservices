using MangaUpdater.Service.Messaging.Services;
using MangaUpdater.Services.Fetcher.Features.Apis;
using MangaUpdater.Services.Fetcher.Features.Factory;
using MangaUpdater.Services.Fetcher.Interfaces;
using MangaUpdater.Services.Fetcher.Services;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<FetcherFactory>();
builder.Services.AddTransient<IFetcher, MangadexApi>();
//builder.Services.AddScoped<IFetcher, AsuraScansScrapper>();

builder.Services.AddTransient<IRabbitMqClient, RabbitMqClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

    return new RabbitMqClient(
        settings.HostName,
        settings.UserName,
        settings.Password,
        settings.Port
    );
});
builder.Services.AddHostedService<GetChaptersBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => 
        options.SwaggerEndpoint("/openapi/v1.json", "MangaUpdater v3"));
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();