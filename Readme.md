# GreenDot Application Insight Logger for .NET Apps


This is the Azure Function test project using .NET SDK for customize logger based on [Application Insights Logger](https://learn.microsoft.com/en-us/azure/azure-monitor/app/ilogger).

## NuGet packages
[GreenDotLogger Package](https://github.com/dchen1-greendotcorp/GreenDotLogger/packages/1658723)
[Code Repo] (https://github.com/dchen1-greendotcorp/GreenDotLogger)

## using GreenDot Application Insight Logger

### 1. Install package ---(cmd) dotnet add [project name] package GreenDotLogger --version 1.0.1

### 2. Implement interface IMaskService, example HttpFunctionAppTest.MaskService.

### 3. If you use MaskService, then implement IMaskHandler, example SSNMaskHandler

### 4. appsettings.json include ApplicationInsights:ConnectionString

### 5. Registeration service, example HttpFunctionAppTest.Startup

### 6. Using logger, example HttpFunctionTest function provide three different ways to log


#Code CertificateTest, CertificateService also show how to download Azure Keyvault certificate and add local x509 store successfully. Code run successfully at docker linux and Azure Docker Container.

#Startup also add AddAzureAppConfiguration, and CertificateService use configuration["aad:clientSecret"] to get value from Azure App Configuration

