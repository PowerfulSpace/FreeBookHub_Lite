# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["PS.FreeBookHub.Gateway.API/PS.FreeBookHub.Gateway.API.csproj", "PS.FreeBookHub.Gateway.API/"]

RUN dotnet restore "PS.FreeBookHub.Gateway.API/PS.FreeBookHub.Gateway.API.csproj"

# Копируем весь исходный код и публикуем
COPY . .
WORKDIR "/src/PS.FreeBookHub.Gateway.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Копируем собранное приложение
COPY --from=build /app/publish .

# Контейнер слушает HTTP на 80
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.FreeBookHub.Gateway.API.dll"]
