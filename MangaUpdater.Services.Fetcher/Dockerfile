# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["MangaUpdater.Services.Fetcher/MangaUpdater.Services.Fetcher.csproj", "./MangaUpdater.Services.Fetcher/"]
RUN dotnet restore "MangaUpdater.Services.Fetcher/MangaUpdater.Services.Fetcher.csproj"

# Copy everything else and build app
COPY . .
WORKDIR "/app/MangaUpdater.Services.Fetcher"
RUN dotnet publish "MangaUpdater.Services.Fetcher.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install Chromium and dependencies
RUN apt-get update && apt-get install -y \
    chromium \
    chromium-driver \
    && rm -rf /var/lib/apt/lists/* 

# Environment variable for Chromium binary location (useful for Selenium/automation tools)
ENV CHROME_BIN=/usr/bin/chromium

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MangaUpdater.Services.Fetcher.dll"]
