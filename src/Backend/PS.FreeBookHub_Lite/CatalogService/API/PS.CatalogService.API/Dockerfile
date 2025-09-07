# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["PS.CatalogService.API/PS.CatalogService.API.csproj", "PS.CatalogService.API/"]
COPY ["PS.CatalogService.Application/PS.CatalogService.Application.csproj", "PS.CatalogService.Application/"]
COPY ["PS.CatalogService.Domain/PS.CatalogService.Domain.csproj", "PS.CatalogService.Domain/"]
COPY ["PS.CatalogService.Infrastructure/PS.CatalogService.Infrastructure.csproj", "PS.CatalogService.Infrastructure/"]
COPY ["PS.CatalogService.Common/PS.CatalogService.Common.csproj", "PS.CatalogService.Common/"]


RUN dotnet restore "PS.CatalogService.API/PS.CatalogService.API.csproj"

# Копируем весь код и публикуем
COPY . .
WORKDIR "/src/PS.CatalogService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.CatalogService.API.dll"]
