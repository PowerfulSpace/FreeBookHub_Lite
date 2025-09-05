# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости (кэшируем restore)
COPY ["PS.FreeBookHub_Lite.CartService.API/PS.FreeBookHub_Lite.CartService.API.csproj", "PS.FreeBookHub_Lite.CartService.API/"]
COPY ["PS.FreeBookHub_Lite.CartService.Application/PS.FreeBookHub_Lite.CartService.Application.csproj", "PS.FreeBookHub_Lite.CartService.Application/"]
COPY ["PS.FreeBookHub_Lite.CartService.Domain/PS.FreeBookHub_Lite.CartService.Domain.csproj", "PS.FreeBookHub_Lite.CartService.Domain/"]
COPY ["PS.FreeBookHub_Lite.CartService.Infrastructure/PS.FreeBookHub_Lite.CartService.Infrastructure.csproj", "PS.FreeBookHub_Lite.CartService.Infrastructure/"]
COPY ["PS.FreeBookHub_Lite.CartService.Common/PS.FreeBookHub_Lite.CartService.Common.csproj", "PS.FreeBookHub_Lite.CartService.Common/"]


RUN dotnet restore "PS.FreeBookHub_Lite.CartService.API/PS.FreeBookHub_Lite.CartService.API.csproj"

# Копируем весь исходный код и публикуем
COPY . .
WORKDIR "/src/PS.FreeBookHub_Lite.CartService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Приложение
COPY --from=build /app/publish .

# Контейнер слушает HTTP на 80
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.FreeBookHub_Lite.CartService.API.dll"]
