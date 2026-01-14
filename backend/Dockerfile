# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Version arguments
ARG APP_VERSION=1.0.0
ARG GIT_COMMIT=unknown

WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["PPM.API/PPM.API.csproj", "PPM.API/"]
COPY ["PPM.Application/PPM.Application.csproj", "PPM.Application/"]
COPY ["PPM.Core/PPM.Core.csproj", "PPM.Core/"]
COPY ["PPM.Infrastructure/PPM.Infrastructure.csproj", "PPM.Infrastructure/"]

RUN dotnet restore "PPM.API/PPM.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/PPM.API"
RUN dotnet build "PPM.API.csproj" -c Release -o /app/build \
    /p:APP_VERSION=${APP_VERSION} /p:GIT_COMMIT=${GIT_COMMIT}

# Publish stage
FROM build AS publish
ARG APP_VERSION=1.0.0
ARG GIT_COMMIT=unknown
RUN dotnet publish "PPM.API.csproj" -c Release -o /app/publish /p:UseAppHost=false \
    /p:APP_VERSION=${APP_VERSION} /p:GIT_COMMIT=${GIT_COMMIT}

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5002

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PPM.API.dll"]
