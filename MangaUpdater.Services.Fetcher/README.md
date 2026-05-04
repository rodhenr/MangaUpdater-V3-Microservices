# MangaUpdater.Services.Fetcher

Fetcher is the runtime worker that consumes chapter fetch jobs, resolves source definitions from the database, executes the correct scraper engine, and publishes normalized chapter results.

## Architecture

- Queue messages identify a source by `SourceId` and optionally `SourceSlug`.
- `DatabaseSourceDefinitionProvider` loads `Source`, request profile, API profile, and scraping profile data from Postgres and caches the resolved `SourceRuntimeDefinition` for 5 minutes.
- `ScraperOrchestrator` builds the `GenericScraperContext`, selects the engine through `SourceEngineResolver`, and converts parsing output into `ChapterResult` items.
- `SourceRequestExecutor` applies request metadata, per-source rate limiting, retry with backoff, and cooldown behavior.
- Queue consumers are reconciled dynamically from active database sources rather than enum registration.

## Engine Types

- `HtmlXPath`: generic HTML fetching and extraction using `SourceScrapingProfile`.
- `JsonApi`: generic API fetching and field extraction using `SourceApiProfile`.
- `Custom`: fallback path for sources that do not fit the generic model. This is currently the intended home for Comick.
- `BrowserHtml` and `BrowserJsonIntercept`: reserved engine names for future browser-driven generic implementations. They are not the current default path.

## Required Configuration

- `ConnectionStrings:DefaultConnection`
- `RabbitMqSettings:Hostname`
- `RabbitMqSettings:Username`
- `RabbitMqSettings:Password`
- `RabbitMqSettings:Port`
- Optional browser configuration for custom engines:
   - `Puppeteer:ExecutablePath`
   - `CHROME_BIN`

## Local Run

1. Start Postgres and RabbitMQ.
2. Apply the latest Database migrations.
3. Ensure the Database service contains the source and profile rows needed by the Fetcher.
4. Set configuration through `appsettings.Development.json`, environment variables, or user secrets.
5. Run `dotnet run --project MangaUpdater.Services.Fetcher`.

## Adding A New Source

1. Create a `Source` record in the Database service with `Name`, `Slug`, `BaseUrl`, `EngineType`, `RequestMode`, `IsEnabled`, queue metadata, and any rate-limit or browser flags.
2. Create one active `SourceRequestProfile` for transport concerns such as `Method`, `UrlTemplate`, headers, timeout, language, and referrer.
3. Create one active parsing profile:
    - `SourceScrapingProfile` for `HtmlXPath`
    - `SourceApiProfile` for `JsonApi`
4. Validate the profile with `POST /api/source/{sourceId}/validate-profile` before enabling production traffic.
5. Enable the source or activate the new profile version. The Database scheduler and Fetcher queue consumers reconcile active sources automatically.

## Engine Semantics

### HtmlXPath

- `ChapterNodesXPath` is required.
- URL extraction can use `ChapterUrlXPath` plus `ChapterUrlAttribute`, or a direct node attribute.
- Chapter number extraction can come from text or an attribute and can be normalized with `ChapterNumberRegex`.
- Date parsing supports exact formats, ISO parsing, and relative date handling.
- Ignore rules are defined with `IgnoreTextContains1/2/3`.
- URL normalization is controlled by `UrlJoinMode` and `UrlPrefix`.
- Deduplication is applied before save-pipeline conversion.

### JsonApi

- `DataRootPath`, `ChapterNumberPath`, and `ChapterDatePath` are required.
- `ChapterUrlPath` and `ResultUrlTemplate` are optional but recommended when a stable public chapter URL exists.
- Pagination is configured through `PaginationMode`, `OffsetParameterName`, `LimitParameterName`, and `NextPagePath`.

### Custom

- Use `Custom` when the site requires browser automation, response interception, or source-specific logic that cannot be represented safely with the generic profile model.
- Keep the custom path narrow and isolated. Prefer moving sources back to generic engines only when their transport and extraction rules are demonstrably stable.

## Generic HTML Example

Simple XPath-oriented configuration example:

```json
{
   "source": {
      "name": "ExampleSource",
      "slug": "examplesource",
      "baseUrl": "https://example.org",
      "engineType": "HtmlXPath",
      "requestMode": "HttpGet",
      "isEnabled": false,
      "supportsPagination": false
   },
   "requestProfile": {
      "isActive": true,
      "version": 1,
      "method": "GET",
      "urlTemplate": "{BaseUrl}{MangaUrlPart}",
      "timeoutSeconds": 30,
      "useCookies": false
   },
   "scrapingProfile": {
      "isActive": true,
      "version": 1,
      "chapterNodesXPath": "//div[contains(@class,'chapter-list')]//a[contains(@href,'/chapter/')]",
      "chapterUrlXPath": ".",
      "chapterUrlAttribute": "href",
      "chapterNumberXPath": ".",
      "chapterNumberRegex": "Chapter\\s+(\\d+(?:\\.\\d+)?)",
      "chapterDateXPath": ".//time",
      "chapterDateAttribute": "datetime",
      "dateParseMode": "IsoDateTime",
      "urlJoinMode": "BaseUrlPrefix",
      "deduplicationKeyMode": "ChapterNumber",
      "chapterSortMode": "NumericAscending",
      "resultLimit": 200
   }
}
```

## When Not To Use The Generic Engine

- The source requires browser event interception or deferred API calls that are not expressible as a request template.
- Chapter URLs depend on client-side state that is not returned in the fetched HTML or API payload.
- Anti-bot measures force a site-specific browser workflow.

## Operational Notes

- Disabled sources are rejected by the Fetcher at definition-load time.
- The Fetcher cache is TTL based only; there is no explicit cross-service cache flush endpoint.
- Broken HTML/API profile configuration opens the same cooldown path as repeated request failures, which prevents the scheduler from hammering the remote site.

See `docs/source-operations.md` for disable, rollback, validation, and cache-handling runbooks.
