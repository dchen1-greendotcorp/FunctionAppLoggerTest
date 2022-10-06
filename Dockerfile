#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FunctionAppLoggerTest.csproj", "."]

RUN dotnet nuget add source --username "dchen1-greendotcorp" --password "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" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/dchen1-greendotcorp/index.json"
RUN dotnet restore "./FunctionAppLoggerTest.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FunctionAppLoggerTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FunctionAppLoggerTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true