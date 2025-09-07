# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["PS.OrderService.API/PS.OrderService.API.csproj", "PS.OrderService.API/"]
COPY ["PS.OrderService.Application/PS.OrderService.Application.csproj", "PS.OrderService.Application/"]
COPY ["PS.OrderService.Domain/PS.OrderService.Domain.csproj", "PS.OrderService.Domain/"]
COPY ["PS.OrderService.Infrastructure/PS.OrderService.Infrastructure.csproj", "PS.OrderService.Infrastructure/"]
COPY ["PS.OrderService.Common/PS.OrderService.Common.csproj", "PS.OrderService.Common/"]


RUN dotnet restore "PS.OrderService.API/PS.OrderService.API.csproj"

COPY . .
WORKDIR "/src/PS.OrderService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.OrderService.API.dll"]
