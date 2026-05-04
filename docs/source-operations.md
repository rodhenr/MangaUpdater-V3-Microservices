# Source Operations

This runbook describes how to add, validate, disable, roll back, and troubleshoot database-driven sources.

## Source Model

Each runtime source is composed of:

- `Source`: operational metadata such as `Name`, `Slug`, `BaseUrl`, `EngineType`, `RequestMode`, `IsEnabled`, `QueueName`, `RateLimitMilliseconds`, and browser capability flags.
- `SourceRequestProfile`: transport settings such as HTTP method, URL template, headers, timeout, language, and referrer.
- `SourceScrapingProfile`: HTML/XPath parsing rules for `HtmlXPath` sources.
- `SourceApiProfile`: JSON/API parsing rules for `JsonApi` sources.

Only one active version per profile type should be considered the runtime profile for a source.

## Create Or Update A Source

1. Create or update the `Source` through `POST /api/source` or `PUT /api/source/{sourceId}`.
2. Add or update the request profile under `api/source/{sourceId}/request-profiles`.
3. Add or update the parsing profile under:
   - `api/source/{sourceId}/scraping-profiles` for `HtmlXPath`
   - `api/source/{sourceId}/api-profiles` for `JsonApi`
4. Keep the source disabled until validation succeeds.

## Validate Before Activation

Use `POST /api/source/{sourceId}/validate-profile` with:

- `targetUrl`: the page or API endpoint to test.
- `requestProfileId`: optional when testing a non-default request profile.
- `apiProfileId` or `scrapingProfileId`: optional when testing a non-default parsing profile.
- `previewLimit`: optional preview size.

Expected behavior:

- The response includes preview chapter candidates.
- Empty XPath or JSON path matches are returned as warnings.
- Failed chapter-number or date parsing is returned as diagnostics.

Recommended activation flow:

1. Create a new inactive or non-production profile version.
2. Validate it against a known manga URL.
3. Promote the profile by marking it active.
4. Enable the source if it was disabled.

## Disable A Broken Source

Use `PUT /api/source/{sourceId}` and set `isEnabled` to `false`.

Effects:

- The Database scheduler stops dispatching new jobs for that source.
- The Fetcher will reject any stale queued work that resolves after the source is disabled.
- Existing queue consumers are reconciled on the next polling cycle.

Use this when the source is failing repeatedly, has changed markup unexpectedly, or should be paused for maintenance.

## Roll Back To A Previous Profile Version

There is no special rollback endpoint. Rollback is done by profile activation state.

1. List the existing profiles for the source.
2. Choose the previous known-good version.
3. Update that profile and set `isActive` to `true`.
4. If needed, set the newer broken profile to `isActive = false`.

The CRUD handlers enforce the single-active-version rule per profile type, so activating one version deactivates the previous active version of the same type.

## Queue Names

- Prefer setting `QueueName` explicitly when compatibility with an existing queue matters.
- If `QueueName` is omitted, both scheduling and consumption fall back to `get-chapters-{slug}`.
- If `Slug` is also empty, the fallback is `get-chapters-{sourceId}`.

## Cache Behavior

- The Fetcher caches resolved source definitions in memory for 5 minutes.
- There is currently no admin-triggered remote cache flush.
- Source changes become visible to the Fetcher after TTL expiry or process restart.

Operational response when a change must apply immediately:

1. Wait for the 5-minute TTL to expire, or
2. Restart the Fetcher service.

## Limits Of The Generic Engine

Choose `Custom` instead of a generic engine when:

- the site requires browser automation or intercepted API traffic,
- the chapter URL cannot be derived from stored HTML/API fields,
- request sequencing or extraction logic depends on source-specific code.

Comick remains the reference example for the custom path.

## Troubleshooting Checklist

1. Confirm the source is enabled.
2. Confirm one active request profile exists.
3. Confirm one active parsing profile exists for the current engine type.
4. Validate the profile against a real target URL.
5. Check queue naming if jobs are not being consumed.
6. Check the Fetcher logs for `FetchConfig` and `Fetch` diagnostics, including source id, engine type, and profile version.
7. If an admin change appears ignored, account for the 5-minute Fetcher cache TTL.