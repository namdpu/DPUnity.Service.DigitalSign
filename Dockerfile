# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Set build args (supplied at build time)
ARG GIT_TOKEN
ARG GIT_USER

# Copy csproj files separately for better caching
COPY DigitalSignService/DigitalSignService.csproj DigitalSignService/
COPY DigitalSignService.Business/DigitalSignService.Business.csproj DigitalSignService.Business/
COPY DigitalSignService.Business/Lib/ DigitalSignService.Business/Lib/
COPY DigitalSignService.Common/DigitalSignService.Common.csproj DigitalSignService.Common/
COPY DigitalSignService.DAL/DigitalSignService.DAL.csproj DigitalSignService.DAL/

# Configure GitHub Packages NuGet source if token provided
RUN if [ ! -z "$GIT_TOKEN" ] && [ ! -z "$GIT_USER" ]; then \
    dotnet nuget add source "https://nuget.pkg.github.com/$GIT_USER/index.json" \
    --name "GitHubPackages" \
    --username "$GIT_USER" \
    --password "$GIT_TOKEN" \
    --store-password-in-clear-text; \
    fi

# Copy packages and NuGet config
COPY DigitalSignService/appsettings.Production.json ./DigitalSignService/

# Restore dependencies
WORKDIR /src/DigitalSignService
RUN dotnet restore DigitalSignService.csproj

# Copy full source and build
COPY . .
COPY DigitalSignService.Business/Lib/ DigitalSignService.Business/Lib/

# Build the Business project
WORKDIR /src/DigitalSignService.Business
RUN dotnet build -c Release

# Publish the main service project
WORKDIR /src/DigitalSignService
RUN dotnet publish DigitalSignService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "DigitalSignService.dll"]
