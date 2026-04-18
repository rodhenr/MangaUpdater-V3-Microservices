# Overview
- **Purpose:** MangaUpdater V3 — a multi-project system to fetch, persist, and serve manga metadata and chapters; connectors to external APIs (e.g., AniList) and internal workers for fetching and processing.
- **Type:** Microservices-oriented solution composed of multiple specialized projects.

# Structure
- **Main folders / projects at repository root:**
	- MangaUpdater.Services.AnilistConnector
	- MangaUpdater.Services.API
	- MangaUpdater.Services.Database
	- MangaUpdater.Services.Fetcher
	- MangaUpdater.Service.Messaging
	- MangaUpdater.Services.Logging
	- MangaUpdater.Shared
- Each service has a `Program.cs`, `Controllers/`, `Features/`, and `Dockerfile` (where applicable).

# Services / Modules
### **MangaUpdater.Services.AnilistConnector**
  - Purpose: lightweight connector to fetch a user's manga collection from Anilist.
  - Entry point: MangaUpdater.Services.AnilistConnector/Program.cs
  - Main controllers: Controllers/UserController.cs
  - Required env vars: none (uses default appsettings)

### **MangaUpdater.Services.API**
  - Purpose: public API aggregating information (user chapters, logs).
  - Entry point: MangaUpdater.Services.API/Program.cs
  - Main controllers: Controllers/InfoController.cs, Controllers/LogController.cs
  - Required env vars:
    - `Database:BaseUrl` (optional, default: http://localhost:5002/)

### **MangaUpdater.Services.Database**
  - Purpose: primary application database and data access layer.
  - Entry point: MangaUpdater.Services.Database/Program.cs
  - Main controllers: Controllers/MangaController.cs, ChaptersController.cs, LogController.cs, etc.
  - Required env vars:
    - `ConnectionStrings:DefaultConnection` (Postgres connection string)
    - `RabbitMqSettings:Hostname`, `RabbitMqSettings:Username`, `RabbitMqSettings:Password`, `RabbitMqSettings:Port`

### **MangaUpdater.Services.Fetcher**
  - Purpose: fetcher/scraper service that retrieves chapters from sources and enqueues work.
  - Entry point: MangaUpdater.Services.Fetcher/Program.cs
  - Main controllers: Controllers/FetcherController.cs, Controllers/ServicesController.cs
  - Required env vars:
    - `ConnectionStrings:DefaultConnection` (used by logging / services)
    - `RabbitMqSettings:Hostname`, `RabbitMqSettings:Username`, `RabbitMqSettings:Password`, `RabbitMqSettings:Port`
    - Note: Puppeteer/Chrome executable is referenced (Dev only: ensure Chrome is available at the configured path)

### **MangaUpdater.Service.Messaging**
  - Purpose: RabbitMQ client implementation used by other services.
  - Files: Services/RabbitMqClient.cs
  - Required env vars: none by itself; consumers should provide `RabbitMqSettings` in their appsettings.

### **MangaUpdater.Services.Logging**
  - Purpose: AppLogger helper used by services to log to the database/storage.
  - Files: AppLogger.cs
  - Required env vars:
    - `ConnectionStrings:DefaultConnection` (used by AppLogger)

# Data Layer
- **Database usage**: a dedicated Database project with `AppDbContext` and `Migrations` indicates use of a relational DB.
- **ORM**: Entity Framework Core is used (inferred from `DbContext` and `Migrations` presence).

# Dependencies
- **External APIs:** AniList (via `AnilistConnector`), other external sources implied by fetcher modules (specific providers: unknown).
- **Internal dependencies:** `MangaUpdater.Shared` used across services; messaging via RabbitMQ client; Docker/Docker Compose for service orchestration.

# Conventions
- **Naming:** project names `MangaUpdater.Services.*`; folders `Controllers`, `Features`, `Services`, `Database`, `DTOs`.
- **Architectural patterns:** microservices with layered organization (Controllers → Services → Database); feature folders for domain separation.
- **Hosting:** each service uses `Program.cs` (host entry); controllers indicate HTTP APIs (MVC-style controllers or minimal API hosting with controllers).

# Observations
- **Missing patterns:** no test projects visible in repository root (tests: unknown).
- **Inconsistencies:** routes and API surface details are not centralized (endpoint shapes: unknown).
- **Technical debt / notes:**
	- Documentation per-service is limited in-code (detailed README or API spec per service: unknown).
	- Some folders show elided content (`...`), so full coverage of files is not available from listing.
	- No visible CI/test configuration in the listed files (CI: unknown).

