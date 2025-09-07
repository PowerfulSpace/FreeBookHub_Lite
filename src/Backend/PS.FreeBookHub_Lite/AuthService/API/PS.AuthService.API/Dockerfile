# ========== 1. Build stage ==========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем CSPROJ файлы и восстанавливаем зависимости
COPY ["PS.AuthService.API/PS.AuthService.API.csproj", "PS.AuthService.API/"]
COPY ["PS.AuthService.Application/PS.AuthService.Application.csproj", "PS.AuthService.Application/"]
COPY ["PS.AuthService.Domain/PS.AuthService.Domain.csproj", "PS.AuthService.Domain/"]
COPY ["PS.AuthService.Infrastructure/PS.AuthService.Infrastructure.csproj", "PS.AuthService.Infrastructure/"]
COPY ["PS.AuthService.Common/PS.AuthService.Common.csproj", "PS.AuthService.Common/"]

RUN dotnet restore "PS.AuthService.API/PS.AuthService.API.csproj"

# Копируем всё остальное и публикуем
COPY . .
WORKDIR "/src/PS.AuthService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ========== 2. Runtime stage ==========
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Копируем собранное приложение
COPY --from=build /app/publish .

# Указываем переменные окружения
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.AuthService.API.dll"]
