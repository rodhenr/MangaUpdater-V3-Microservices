# Build stage using .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

# Set working directory
WORKDIR /app

# Copy csproj file and restore dependencies
COPY MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj ./MangaUpdater.Services.Database/
RUN dotnet restore ./MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj

# Copy the rest of the application code
COPY . .

# Set working directory to the project
WORKDIR /app/MangaUpdater.Services.Database

# Install the EF Core CLI tools
RUN dotnet tool install --global dotnet-ef --version 9.0.0

# Ensure the .NET tools are available globally
ENV PATH="$PATH:/root/.dotnet/tools"

# Run migrations
RUN dotnet ef database update --connection "Host=postgres;Port=5432;Database=MangaUpdater;Username=usuario;Password=123456789!" --project /app/MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj --startup-project /app/MangaUpdater.Services.Database/MangaUpdater.Services.Database.csproj
