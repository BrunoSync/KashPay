FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ ./src/

RUN dotnet restore src/KashPay.Api/KashPay.Api.csproj

RUN dotnet publish src/KashPay.Api/KashPay.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KashPay.Api.dll"]