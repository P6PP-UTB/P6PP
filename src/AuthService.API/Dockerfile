FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8005
#EXPOSE 8081
COPY certs/DigiCertGlobalRootG2.crt.pem /app/certs/

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AuthService.API/AuthService.API.csproj", "src/AuthService.API/"]
COPY ["libs/ReservationSystem.Shared/ReservationSystem.Shared.csproj", "libs/ReservationSystem.Shared/"]
RUN dotnet restore "src/AuthService.API/AuthService.API.csproj"
COPY . .
WORKDIR "/src/src/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=base /app/certs/DigiCertGlobalRootG2.crt.pem /app/certs/
ENTRYPOINT ["dotnet", "AuthService.API.dll"]

# Spuštění migrací před spuštěním aplikace
#CMD ["sh", "-c", "dotnet AuthService.API.dll && dotnet ef database update"]