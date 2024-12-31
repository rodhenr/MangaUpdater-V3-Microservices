# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["MangaUpdater.Services.API/MangaUpdater.Services.API.csproj", "./MangaUpdater.Services.API/"]
RUN dotnet restore "MangaUpdater.Services.API/MangaUpdater.Services.API.csproj"

# Copy everything else and build app
COPY . . 
WORKDIR "/app/MangaUpdater.Services.API"
RUN dotnet publish "MangaUpdater.Services.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MangaUpdater.Services.API.dll"]