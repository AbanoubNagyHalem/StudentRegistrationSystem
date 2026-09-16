# ============================================================
# Build Stage
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore (better layer caching)
COPY ["StudentRegistration.Api/StudentRegistration.Api.csproj", "StudentRegistration.Api/"]
COPY ["StudentRegistration.Application/StudentRegistration.Application.csproj", "StudentRegistration.Application/"]
COPY ["StudentRegistration.Domain/StudentRegistration.Domain.csproj", "StudentRegistration.Domain/"]
COPY ["StudentRegistration.Infrastructure/StudentRegistration.Infrastructure.csproj", "StudentRegistration.Infrastructure/"]

RUN dotnet restore "StudentRegistration.Api/StudentRegistration.Api.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/StudentRegistration.Api"
RUN dotnet build "StudentRegistration.Api.csproj" -c Release -o /app/build

# ============================================================
# Publish Stage
# ============================================================
FROM build AS publish
RUN dotnet publish "StudentRegistration.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================================
# Runtime Stage
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "StudentRegistration.Api.dll"]