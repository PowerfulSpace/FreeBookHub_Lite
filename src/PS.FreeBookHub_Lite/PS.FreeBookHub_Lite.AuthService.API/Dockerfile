# ========== 1. Build stage ==========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем CSPROJ файлы и восстанавливаем зависимости
COPY ["PS.FreeBookHub_Lite.AuthService.API/PS.FreeBookHub_Lite.AuthService.API.csproj", "PS.FreeBookHub_Lite.AuthService.API/"]
COPY ["PS.FreeBookHub_Lite.AuthService.Application/PS.FreeBookHub_Lite.AuthService.Application.csproj", "PS.FreeBookHub_Lite.AuthService.Application/"]
COPY ["PS.FreeBookHub_Lite.AuthService.Domain/PS.FreeBookHub_Lite.AuthService.Domain.csproj", "PS.FreeBookHub_Lite.AuthService.Domain/"]
COPY ["PS.FreeBookHub_Lite.AuthService.Infrastructure/PS.FreeBookHub_Lite.AuthService.Infrastructure.csproj", "PS.FreeBookHub_Lite.AuthService.Infrastructure/"]
COPY ["PS.FreeBookHub_Lite.AuthService.Common/PS.FreeBookHub_Lite.AuthService.Common.csproj", "PS.FreeBookHub_Lite.AuthService.Common/"]

RUN dotnet restore "PS.FreeBookHub_Lite.AuthService.API/PS.FreeBookHub_Lite.AuthService.API.csproj"

# Копируем всё остальное и публикуем
COPY . .
WORKDIR "/src/PS.FreeBookHub_Lite.AuthService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ========== 2. Runtime stage ==========
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Копируем собранное приложение
COPY --from=build /app/publish .

# Указываем переменные окружения
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.FreeBookHub_Lite.AuthService.API.dll"]
