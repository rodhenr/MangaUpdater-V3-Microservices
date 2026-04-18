MangaUpdater.Service.Messaging

Purpose: RabbitMQ client helper library used by services to publish/consume messages.

Notes: This is a library and not an HTTP service. It expects consuming projects to configure `RabbitMqSettings` in their configuration (see Database and Fetcher).

How to use locally:

1. Consumers should provide the configuration section `RabbitMqSettings` in `appsettings.Development.json` or environment variables.
