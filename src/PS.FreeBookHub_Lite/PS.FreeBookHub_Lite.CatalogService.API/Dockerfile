# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["PS.FreeBookHub_Lite.CatalogService/PS.FreeBookHub_Lite.CatalogService.csproj", "PS.FreeBookHub_Lite.CatalogService/"]
COPY ["PS.FreeBookHub_Lite.CatalogService.Application/PS.FreeBookHub_Lite.CatalogService.Application.csproj", "PS.FreeBookHub_Lite.CatalogService.Application/"]
COPY ["PS.FreeBookHub_Lite.CatalogService.Infrastructure/PS.FreeBookHub_Lite.CatalogService.Infrastructure.csproj", "PS.FreeBookHub_Lite.CatalogService.Infrastructure/"]
COPY ["PS.FreeBookHub_Lite.CatalogService.Common/PS.FreeBookHub_Lite.CatalogService.Common.csproj", "PS.FreeBookHub_Lite.CatalogService.Common/"]

RUN dotnet restore "PS.FreeBookHub_Lite.CatalogService/PS.FreeBookHub_Lite.CatalogService.csproj"

# Копируем весь код и публикуем
COPY . .
WORKDIR "/src/PS.FreeBookHub_Lite.CatalogService"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.FreeBookHub_Lite.CatalogService.dll"]
