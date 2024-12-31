# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["MangaUpdater.Services.AnilistConnector/MangaUpdater.Services.AnilistConnector.csproj", "./MangaUpdater.Services.AnilistConnector/"]
RUN dotnet restore "MangaUpdater.Services.AnilistConnector/MangaUpdater.Services.AnilistConnector.csproj"

# Copy everything else and build app
COPY . . 
WORKDIR "/app/MangaUpdater.Services.AnilistConnector"
RUN dotnet publish "MangaUpdater.Services.AnilistConnector.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MangaUpdater.Services.AnilistConnector.dll"]