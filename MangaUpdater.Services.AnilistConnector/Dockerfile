FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

# Initial configuration
WORKDIR /app
COPY ../*.sln .
COPY ../MangaUpdater.Shared/MangaUpdater.Shared.csproj ../MangaUpdater.Shared/
COPY . .

# Restore packages && Publish the application
WORKDIR /app/
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose the port your application will run on
EXPOSE 8080

ENTRYPOINT ["dotnet", "MangaUpdater.Services.AnilistConnector.dll"]