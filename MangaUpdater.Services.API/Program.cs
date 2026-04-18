using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Built-in services
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

// Named HttpClient for internal Database service (configure Database:BaseUrl in appsettings)
builder.Services.AddHttpClient("Database", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["Database:BaseUrl"] ?? "http://localhost:5002/";
    client.BaseAddress = new Uri(baseUrl);
});

// Mediatr
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
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();

// Add CorrelationId middleware before controllers
app.UseMiddleware<MangaUpdater.Shared.Middlewares.CorrelationIdMiddleware>();

app.MapControllers();

app.Run();