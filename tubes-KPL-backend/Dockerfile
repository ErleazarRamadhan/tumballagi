FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tubes-KPL-backend/tubes-KPL-backend.csproj", "tubes-KPL-backend/"]
RUN dotnet restore "tubes-KPL-backend/tubes-KPL-backend.csproj"
COPY . .
WORKDIR "/src/tubes-KPL-backend"
RUN dotnet build "./tubes-KPL-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./tubes-KPL-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tubes-KPL-backend.dll"]
