MangaUpdater.Services.Fetcher

Purpose: Fetcher/scraper service that retrieves chapter lists from sources.

Required environment variables / configuration:
- `ConnectionStrings:DefaultConnection` — used by logging and services
- `RabbitMqSettings:Hostname`, `RabbitMqSettings:Username`, `RabbitMqSettings:Password`, `RabbitMqSettings:Port`

Notes:
- The fetcher uses PuppeteerSharp and expects a Chrome/Chromium executable. The project config references `/usr/bin/google-chrome` by default (adjust for local Windows paths in development).

How to run locally:

1. Ensure RabbitMQ and Postgres are running and update `appsettings.Development.json` or environment variables.
2. Run:

   dotnet run --project MangaUpdater.Services.Fetcher
