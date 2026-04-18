MangaUpdater.Services.Logging

Purpose: small logging helper used by services to persist logs (AppLogger).

Required environment variables:
- `ConnectionStrings:DefaultConnection` — used by `AppLogger` to persist logs.

How to run locally:

This project is a helper library. Ensure the consuming service sets `ConnectionStrings:DefaultConnection` and references `MangaUpdater.Services.Logging` when running.
