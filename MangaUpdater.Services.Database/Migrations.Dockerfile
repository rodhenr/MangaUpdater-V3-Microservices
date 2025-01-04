# Use .NET SDK (Alpine version) for migrations
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS migrations

# Set working directory
WORKDIR /app

# Copy csproj file and restore dependencies
COPY MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj MangaUpdater.Services.Database/
RUN dotnet restore MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj

# Copy the rest of the application code
COPY . .

# Set working directory to the project folder
WORKDIR /app/MangaUpdater.Services.Database

# Install dotnet-ef globally
RUN dotnet tool install --global dotnet-ef

# Ensure the PATH includes the location of global tools
ENV PATH="$PATH:/root/.dotnet/tools"