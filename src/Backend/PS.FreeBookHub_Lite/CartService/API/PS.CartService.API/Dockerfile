# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости (кэшируем restore)
COPY ["PS.CartService.API/PS.CartService.API.csproj", "PS.CartService.API/"]
COPY ["PS.CartService.Application/PS.CartService.Application.csproj", "PS.CartService.Application/"]
COPY ["PS.CartService.Domain/PS.CartService.Domain.csproj", "PS.CartService.Domain/"]
COPY ["PS.CartService.Infrastructure/PS.CartService.Infrastructure.csproj", "PS.CartService.Infrastructure/"]
COPY ["PS.CartService.Common/PS.CartService.Common.csproj", "PS.CartService.Common/"]


RUN dotnet restore "PS.CartService.API/PS.CartService.API.csproj"

# Копируем весь исходный код и публикуем
COPY . .
WORKDIR "/src/PS.CartService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Приложение
COPY --from=build /app/publish .

# Контейнер слушает HTTP на 80
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.CartService.API.dll"]
