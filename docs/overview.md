# Overview

MangaUpdater V3 is a microservices-oriented solution that fetches manga chapters from external sources, stores normalized data in Postgres, and exposes that data through service APIs. The fetch pipeline is now source-id and database driven: source runtime metadata, request rules, and parsing profiles are stored in the database instead of being selected from source-specific enums.

# Repository Structure

- MangaUpdater.Services.Database: primary application data service, source configuration CRUD, scheduling control, and validation endpoints.
- MangaUpdater.Services.Fetcher: runtime scraper worker that resolves source definitions from the database and executes the configured engine.
- MangaUpdater.Services.API: public API facade over application data.
- MangaUpdater.Services.AnilistConnector: AniList integration for user manga collection import.
- MangaUpdater.Service.Messaging: RabbitMQ client wrapper shared by services.
- MangaUpdater.Services.Logging: shared application logger.
- MangaUpdater.Shared: DTOs, request models, shared enums, helpers, and runtime contracts.

# Runtime Architecture

- Source identity is carried by `SourceId` plus optional `SourceSlug` in queue messages.
- The Database service stores source runtime metadata in `Source`, `SourceRequestProfile`, `SourceApiProfile`, and `SourceScrapingProfile`.
- The Fetcher resolves one consolidated `SourceRuntimeDefinition` from the database and caches it for 5 minutes.
- `HtmlXPath` and `JsonApi` are the generic engines currently used for migrated sources.
- `Custom` remains the escape hatch for sources that still require dedicated code, such as the browser-driven Comick path.
- Queue topology is also database driven. A source can provide an explicit `QueueName`; otherwise the services fall back to `get-chapters-{slug}` or `get-chapters-{sourceId}`.

# Services

## MangaUpdater.Services.Database

- Purpose: EF Core data access, source/admin APIs, dynamic source scheduling, and validation tooling.
- Entry point: `MangaUpdater.Services.Database/Program.cs`
- Main source endpoints: `api/source`, `api/source/{sourceId}/request-profiles`, `api/source/{sourceId}/api-profiles`, `api/source/{sourceId}/scraping-profiles`, `api/source/{sourceId}/validate-profile`
- Required configuration:
  - `ConnectionStrings:DefaultConnection`
  - `RabbitMqSettings:Hostname`
  - `RabbitMqSettings:Username`
  - `RabbitMqSettings:Password`
  - `RabbitMqSettings:Port`

## MangaUpdater.Services.Fetcher

- Purpose: consume chapter fetch jobs, resolve active source definitions from the database, execute engines, and publish normalized chapter results.
- Entry point: `MangaUpdater.Services.Fetcher/Program.cs`
- Main endpoints: `api/fetcher`, `api/services/queue`, `api/services/queue/pause`, `api/services/queue/resume`
- Required configuration:
  - `ConnectionStrings:DefaultConnection`
  - `RabbitMqSettings:Hostname`
  - `RabbitMqSettings:Username`
  - `RabbitMqSettings:Password`
  - `RabbitMqSettings:Port`
  - Optional browser config for custom sources: `Puppeteer:ExecutablePath` or `CHROME_BIN`

## MangaUpdater.Services.API

- Purpose: public API over manga, chapter, and log data.
- Entry point: `MangaUpdater.Services.API/Program.cs`
- Required configuration:
  - `Microservices:Database`

## MangaUpdater.Services.AnilistConnector

- Purpose: AniList collection import.
- Entry point: `MangaUpdater.Services.AnilistConnector/Program.cs`

## Shared Support Projects

- MangaUpdater.Service.Messaging: RabbitMQ infrastructure.
- MangaUpdater.Services.Logging: application logging implementation.
- MangaUpdater.Shared: shared contracts used across services.

# Conventions

- Business logic lives in `Features` or services. Controllers remain thin and delegate to MediatR handlers or orchestrators.
- Source/profile versioning is activation based: one active version per profile type per source.
- The Fetcher prefers consistency with the configured database profile over hardcoded source branches.

# Related Docs

- `docs/source-operations.md`: source onboarding, validation, rollback, disable flows, and cache behavior.
- `MangaUpdater.Services.Fetcher/README.md`: engine semantics, setup steps, and generic HTML examples.

