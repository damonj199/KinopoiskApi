FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/KinopoikDB.Api/KinopoikDB.Api.csproj", "src/KinopoikDB.Api/"]
COPY ["src/KinopoiskDB.Infrastructure/KinopoiskDB.Infrastructure.csproj", "src/KinopoiskDB.Infrastructure/"]
COPY ["src/KinopoiskDB.Application/KinopoiskDB.Application.csproj", "src/KinopoiskDB.Application/"]
COPY ["src/KinopoiskDB.Core/KinopoiskDB.Core.csproj", "src/KinopoiskDB.Core/"]
COPY ["src/KinopoiskDB.Dal.PostgreSQL/KinopoiskDB.Dal.PostgreSQL.csproj", "src/KinopoiskDB.Dal.PostgreSQL/"]
RUN dotnet restore "./src/KinopoikDB.Api/KinopoikDB.Api.csproj"
COPY . .
WORKDIR "/src/src/KinopoikDB.Api"
RUN dotnet build "./KinopoikDB.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KinopoikDB.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KinopoikDB.Api.dll"]