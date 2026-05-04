using MangaUpdater.Services.Fetcher;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);
var configuredUrls = builder.Configuration[WebHostDefaults.ServerUrlsKey];
var hasHttpsBinding = configuredUrls?
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) == true;

// Built-in services
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddCors();
builder.Services.AddOpenApi();

// Custom services
builder.Services.AddServices(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "MangaUpdater v3"));
}

// Built-in
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
if (hasHttpsBinding)
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();

// Correlation middleware
app.UseMiddleware<MangaUpdater.Shared.Middlewares.CorrelationIdMiddleware>();

app.MapControllers();

app.Run();