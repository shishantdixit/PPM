# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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
RUN dotnet build "PPM.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PPM.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PPM.API.dll"]
