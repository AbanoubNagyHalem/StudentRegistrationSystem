# ============================================================
# Build Stage
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore (better layer caching)
COPY ["StudentRegistration.Client/StudentRegistration.Client.csproj", "StudentRegistration.Client/"]

RUN dotnet restore "StudentRegistration.Client/StudentRegistration.Client.csproj"

# Copy everything and publish
COPY . .
WORKDIR "/src/StudentRegistration.Client"
RUN dotnet publish "StudentRegistration.Client.csproj" -c Release -o /app/publish

# ============================================================
# Serve Stage (nginx)
# ============================================================
FROM nginx:alpine AS final

RUN rm -rf /usr/share/nginx/html/*

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY StudentRegistration.Client/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]