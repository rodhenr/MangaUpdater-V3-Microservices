using MangaUpdater.Shared.Middlewares;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Built-in
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("Database", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl =
        config["Microservices:Database"] ??
        config["Database:BaseUrl"] ??
        "http://localhost:5002/";
    client.BaseAddress = new Uri(baseUrl);
});

var executingAssembly = Assembly.GetExecutingAssembly();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(executingAssembly);
});

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

// Correlation middleware
app.UseMiddleware<MangaUpdater.Shared.Middlewares.CorrelationIdMiddleware>();

app.MapControllers();

// Middlewares
app.UseMiddleware<HttpExceptionMiddleware>();

app.Run();