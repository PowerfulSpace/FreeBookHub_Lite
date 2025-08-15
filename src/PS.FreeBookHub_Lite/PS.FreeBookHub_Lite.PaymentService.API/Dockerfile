# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["PS.FreeBookHub_Lite.PaymentService.API/PS.FreeBookHub_Lite.PaymentService.API.csproj", "PS.FreeBookHub_Lite.PaymentService.API/"]
COPY ["PS.FreeBookHub_Lite.PaymentService.Application/PS.FreeBookHub_Lite.PaymentService.Application.csproj", "PS.FreeBookHub_Lite.PaymentService.Application/"]
COPY ["PS.FreeBookHub_Lite.PaymentService.Domain/PS.FreeBookHub_Lite.PaymentService.Domain.csproj", "PS.FreeBookHub_Lite.PaymentService.Domain/"]
COPY ["PS.FreeBookHub_Lite.PaymentService.Infrastructure/PS.FreeBookHub_Lite.PaymentService.Infrastructure.csproj", "PS.FreeBookHub_Lite.PaymentService.Infrastructure/"]
COPY ["PS.FreeBookHub_Lite.PaymentService.Common/PS.FreeBookHub_Lite.PaymentService.Common.csproj", "PS.FreeBookHub_Lite.PaymentService.Common/"]

RUN dotnet restore "PS.FreeBookHub_Lite.PaymentService.API/PS.FreeBookHub_Lite.PaymentService.API.csproj"

COPY . .
WORKDIR "/src/PS.FreeBookHub_Lite.PaymentService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.FreeBookHub_Lite.PaymentService.API.dll"]
