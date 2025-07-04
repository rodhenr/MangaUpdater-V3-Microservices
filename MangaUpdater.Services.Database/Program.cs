using MangaUpdater.Services.Database;
using MangaUpdater.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Built-in services
builder.Services.AddControllers();
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
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Middlewares
app.UseMiddleware<HttpExceptionMiddleware>();

app.Run();