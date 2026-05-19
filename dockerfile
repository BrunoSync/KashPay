FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ./src/KashPay.Api/KashPay.Api.csproj ./src/KashPay.Api/KashPay.Api.csproj
COPY ./src/KashPay.Domain/KashPay.Domain.csproj ./src/KashPay.Domain/KashPay.Domain.csproj
COPY ./src/KashPay.Application/KashPay.Application.csproj ./src/KashPay.Application/KashPay.Application.csproj
COPY ./src/KashPay.Infrastructure/KashPay.Infrastructure.csproj ./src/KashPay.Infrastructure/KashPay.Infrastructure.csproj

RUN dotnet restore ./src/KashPay.Api/KashPay.Api.csproj
COPY . .

RUN dotnet publish src/KashPay.Api/KashPay.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KashPay.Api.dll"]