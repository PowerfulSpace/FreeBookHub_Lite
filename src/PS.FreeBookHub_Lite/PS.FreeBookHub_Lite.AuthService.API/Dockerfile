# ========== 1. Build stage ==========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем CSPROJ файлы и восстанавливаем зависимости
COPY ["PS.FreeBookHub_Lite.AuthService/PS.FreeBookHub_Lite.AuthService.csproj", "PS.FreeBookHub_Lite.AuthService/"]
COPY ["PS.FreeBookHub_Lite.AuthService.Application/PS.FreeBookHub_Lite.AuthService.Application.csproj", "PS.FreeBookHub_Lite.AuthService.Application/"]
COPY ["PS.FreeBookHub_Lite.AuthService.Infrastructure/PS.FreeBookHub_Lite.AuthService.Infrastructure.csproj", "PS.FreeBookHub_Lite.AuthService.Infrastructure/"]

RUN dotnet restore "PS.FreeBookHub_Lite.AuthService/PS.FreeBookHub_Lite.AuthService.csproj"

# Копируем всё остальное и публикуем
COPY . .
WORKDIR "/src/PS.FreeBookHub_Lite.AuthService"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ========== 2. Runtime stage ==========
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Копируем собранное приложение
COPY --from=build /app/publish .

# Указываем переменные окружения
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.FreeBookHub_Lite.AuthService.dll"]
