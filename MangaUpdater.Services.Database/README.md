MangaUpdater.Services.Database

Purpose: Primary data service — EF Core DbContext, entities, and data access.

Required environment variables / configuration:
- `ConnectionStrings:DefaultConnection` — Postgres connection string (e.g. `Host=localhost;Database=manga;Username=...;Password=...`)
- `RabbitMqSettings:Hostname`, `RabbitMqSettings:Username`, `RabbitMqSettings:Password`, `RabbitMqSettings:Port`

How to run locally:

1. Ensure Postgres is available and set `ConnectionStrings:DefaultConnection` in `appsettings.Development.json` or environment variables.
2. (Optional) Apply migrations locally:

   dotnet ef database update --project MangaUpdater.Services.Database --startup-project MangaUpdater.Services.Database

3. Run the service:

   dotnet run --project MangaUpdater.Services.Database
