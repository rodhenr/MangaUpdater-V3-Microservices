# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj", "./MangaUpdater.Services.Database/"]
RUN dotnet restore "MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj"

# Copy everything else and build app
COPY . . 
WORKDIR "/app/MangaUpdater.Services.Database"
RUN dotnet publish "MangaUpdater.Services.Database.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MangaUpdater.Services.Database.dll"]
