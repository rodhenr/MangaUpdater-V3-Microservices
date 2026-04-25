using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Built-in services
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Named HttpClient for internal Database service.
builder.Services.AddHttpClient("Database", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl =
        config["Microservices:Database"] ??
        config["Database:BaseUrl"] ??
        "http://localhost:5002/";
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
app.UseMiddleware<MangaUpdater.Shared.Middlewares.HttpExceptionMiddleware>();

app.MapControllers();

app.Run();