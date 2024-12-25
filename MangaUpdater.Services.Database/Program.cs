using MangaUpdater.Service.Messaging.Services;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.Chapters;
using MangaUpdater.Services.Database.Services;
using MangaUpdater.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IRabbitMqClient, RabbitMqClient>();
builder.Services.AddHostedService<SaveChapterBackgroundService>();
builder.Services.AddScoped<ISaveChapters, SaveChapters>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => 
        options.SwaggerEndpoint("/openapi/v1.json", "MangaUpdater v3"));
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();