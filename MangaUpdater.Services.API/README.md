MangaUpdater.Services.API

Purpose: Public API that aggregates information (user chapters, logs).

Required environment variables:
- `Database:BaseUrl` - optional, the base URL used to contact the Database service (default: http://localhost:5002/)

How to run locally:

1. Ensure the Database service is running or set `Database:BaseUrl` to a running database API.
2. From repository root run:

   dotnet run --project MangaUpdater.Services.API

3. Development OpenAPI is available when the app is run in Development environment.
