# ===== 1) Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["PS.PaymentService.API/PS.PaymentService.API.csproj", "PS.PaymentService.API/"]
COPY ["PS.PaymentService.Application/PS.PaymentService.Application.csproj", "PS.PaymentService.Application/"]
COPY ["PS.PaymentService.Domain/PS.PaymentService.Domain.csproj", "PS.PaymentService.Domain/"]
COPY ["PS.PaymentService.Infrastructure/PS.PaymentService.Infrastructure.csproj", "PS.PaymentService.Infrastructure/"]
COPY ["PS.PaymentService.Common/PS.PaymentService.Common.csproj", "PS.PaymentService.Common/"]


RUN dotnet restore "PS.PaymentService.API/PS.PaymentService.API.csproj"

COPY . .
WORKDIR "/src/PS.PaymentService.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== 2) Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "PS.PaymentService.API.dll"]
