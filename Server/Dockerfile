FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server/WebApp.Server.csproj", "Server/"]
COPY ["Client/WebApp.Client.csproj", "Client/"]
COPY ["Common/WebApp.Common.csproj", "Common/"]
COPY ["Database/WebApp.Database.csproj", "Database/"]
COPY ["External Integrations/ESPN/ESPN.Service/ESPN.Service.csproj", "External Integrations/ESPN/ESPN.Service/"]
RUN dotnet restore "./Server/WebApp.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "./WebApp.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApp.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_GCServer=1
ENV ASPNETCORE_URLS=https://+:8081;http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl --fail http://localhost:8080/api/healthchecks || exit 1
ENTRYPOINT ["dotnet", "WebApp.Server.dll"]