using MangaUpdater.Services.Fetcher.Features.Apis;
using MangaUpdater.Services.Fetcher.Features.Factory;
using MangaUpdater.Services.Fetcher.Features.Scrapers;
using MangaUpdater.Services.Fetcher.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();
//builder.Services.AutoRegister();

builder.Services.AddSingleton<FetcherFactory>();
builder.Services.AddTransient<IFetcher, MangadexApi>();
//builder.Services.AddScoped<IFetcher, AsuraScansScrapper>();

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