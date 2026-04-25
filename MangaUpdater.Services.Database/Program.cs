using System.Text;
using MangaUpdater.Services.Database;
using MangaUpdater.Shared.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Built-in services
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddOpenApi();

// Custom services
builder.Services.AddServices(builder.Configuration);

// Authentication (JWT) - minimal admin user support
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-change-me";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

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

// Add authentication/authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Middlewares
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<HttpExceptionMiddleware>();

app.Run();