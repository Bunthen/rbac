FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DevOpsSample/DevOpsSample.csproj", "DevOpsSample/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "DevOpsSample/DevOpsSample.csproj"

COPY . .
WORKDIR "/src/DevOpsSample"
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet build "DevOpsSample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "DevOpsSample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevOpsSample.dll"]

