using MangaUpdater.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Built-in
builder.Services.AddOpenApi();
builder.Services.AddControllers();

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
app.UseHttpsRedirection();
app.MapControllers();

// Middlewares
app.UseMiddleware<HttpExceptionMiddleware>();

app.Run();