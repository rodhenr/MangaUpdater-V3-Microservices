using System.Reflection;
using MangaUpdater.Service.Messaging.Services;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Services.Database.Feature.Chapters;
using MangaUpdater.Services.Database.Services;
using MangaUpdater.Shared.Interfaces;
using MangaUpdater.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));

builder.Services.AddTransient<IRabbitMqClient, RabbitMqClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

    return new RabbitMqClient(
        settings.Hostname,
        settings.Username,
        settings.Password,
        settings.Port
    );
});

builder.Services.AddScoped<ISaveChapters, SaveChapters>();

builder.Services.AddHostedService<GetChaptersBackgroundService>();
builder.Services.AddHostedService<SaveChapterBackgroundService>();

var executingAssembly = Assembly.GetExecutingAssembly();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(executingAssembly);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => 
        options.SwaggerEndpoint("/openapi/v1.json", "MangaUpdater v3"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();