FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG APP_UID=1000
RUN adduser --disabled-password --uid ${APP_UID} appuser
USER ${APP_UID}
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files first (for layer caching)
COPY cmedcc-idass.sln ./
COPY cmedcc-idass.backend/cmedcc-idass.backend.csproj ./cmedcc-idass.backend/

# Restore dependencies
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "cmedcc-idass.backend/cmedcc-idass.backend.csproj"

# Copy the rest of the source
COPY . .

# Build
WORKDIR /src/cmedcc-idass.backend
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet build "cmedcc-idass.backend.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "cmedcc-idass.backend.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cmedcc-idass.backend.dll"]
