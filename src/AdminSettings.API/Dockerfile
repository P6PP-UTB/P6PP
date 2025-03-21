FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ✅ Copy csproj soubory
COPY ["src/AdminSettings.API/AdminSettings.API.csproj", "src/AdminSettings.API/"]
COPY ["libs/ReservationSystem.Shared/ReservationSystem.Shared.csproj", "libs/ReservationSystem.Shared/"]

# ✅ Restore
RUN dotnet restore "src/AdminSettings.API/AdminSettings.API.csproj"

# ✅ Copy zbytek
COPY . .

WORKDIR "/src/src/AdminSettings.API"
RUN dotnet build "AdminSettings.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AdminSettings.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminSettings.API.dll"]
