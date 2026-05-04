
# MangaUpdater V3 — Microservices

Small set of microservices and helpers to fetch, normalize and store manga chapter information from multiple sources.

**Contents**

- `MangaUpdater.Services.Database` — primary data service (EF Core + Postgres).
- `MangaUpdater.Services.API` — public API aggregating information and exposing endpoints.
- `MangaUpdater.Services.Fetcher` — fetcher and scrapers that collect chapters from sources.
- `MangaUpdater.Services.AnilistConnector` — connector to retrieve user collections from Anilist.
- `MangaUpdater.Service.Messaging` — RabbitMQ client helper.
- `MangaUpdater.Services.Logging` — logging helper used by services.

**Quick links**

- Services overview: [docs/overview.md](docs/overview.md#L1)
- Source operations: [docs/source-operations.md](docs/source-operations.md#L1)
- Project solution: `MangaUpdater_V3.sln`
- Docker compose: `docker-compose.yml`

## Quick start (local)

Prerequisites: .NET SDK, Docker (optional), Postgres and RabbitMQ for the full stack.

Run an individual service from the repository root:

```bash
dotnet run --project MangaUpdater.Services.Database
dotnet run --project MangaUpdater.Services.API
dotnet run --project MangaUpdater.Services.Fetcher
dotnet run --project MangaUpdater.Services.AnilistConnector
```

Or start the recommended local stack with Docker (if configured):

```bash
docker-compose up -d
```

Check `docs/overview.md` for required environment variables and service boundaries.
Check `docs/source-operations.md` for source onboarding, validation, rollback, and cache behavior.

## Contributing

Small, focused changes are preferred. Follow the TODO plan in `TODO.md` for guided improvements. When adding config secrets use `dotnet user-secrets` for local development.

## License

See repository for license information. If none is present, ask the maintainers to clarify.

