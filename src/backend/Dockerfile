# ==============================================================================
# Dockerfile Multi-Stage para ASP.NET Core 8 Web API
# Optimizado para AWS ECS Fargate: Imagen ligera, usuario no-root en puerto 8080
# ==============================================================================

# ------------------------------------------------------------------------------
# Etapa 1: Runtime Base (Alpine Linux + ASP.NET Core 8 Runtime)
# ------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    ASPNETCORE_HTTP_PORTS=8080

WORKDIR /app
EXPOSE 8080

# Usuario sin privilegios root para cumplimiento de seguridad en AWS Fargate
USER $APP_UID

# ------------------------------------------------------------------------------
# Etapa 2: Build SDK (Restauración y Compilación)
# ------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos .csproj para aprovechar la caché de capas de Docker en restore
COPY ["Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["Core/Application/Application.csproj", "Core/Application/"]
COPY ["Adapters/Infrastructure/Infrastructure.csproj", "Adapters/Infrastructure/"]
COPY ["Adapters/Api/Api.csproj", "Adapters/Api/"]

# Restaurar dependencias
RUN dotnet restore "Adapters/Api/Api.csproj"

# Copiar el resto del código fuente del backend
COPY . .

# Compilar proyecto API
WORKDIR "/src/Adapters/Api"
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ------------------------------------------------------------------------------
# Etapa 3: Publicación optimizada (Release)
# ------------------------------------------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ------------------------------------------------------------------------------
# Etapa 4: Imagen Final de Producción
# ------------------------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
