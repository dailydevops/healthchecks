# HealthChecks

![GitHub](https://img.shields.io/github/license/dailydevops/healthchecks?logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/dailydevops/healthchecks?logo=github)
![GitHub repo size](https://img.shields.io/github/repo-size/dailydevops/healthchecks?logo=github)
[![GitHub Pipeline CI](https://github.com/dailydevops/healthchecks/actions/workflows/cicd.yml/badge.svg?branch=main&event=push)](https://github.com/dailydevops/healthchecks/actions/workflows/cicd.yml)

![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/dailydevops/healthchecks/main?logo=codefactor)
![Codecov](https://img.shields.io/codecov/c/github/dailydevops/healthchecks?logo=codecov)

## What is HealthChecks?
This is a mono repository for several NuGet packages based on the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks) package. The main goal of this repository is to provide a set of health checks for different services and frameworks, which are fully configurable either via code or configuration.

## What is the difference between this repository and the [AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) repository?
The main difference is that we try to focus on delivering packages, which are fully configurable via code or configuration. This means that you can configure the health checks in your `Startup.cs` file or in your `appsettings.json` file, or any other configuration provider.

## Available NuGet packages
<!--- start packages -->
| Package Name | NuGet | Description |
|--------------|-------|-------------|
<!--- end packages -->

## Supported .NET version
We try to support the latest LTS and STS versions of .NET ([.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)), as well as the latest preview version of .NET. We will try to support each framework version for at least 3 years, but we can't guarantee it. This depends on the support of related NuGet packages and the .NET platform itself.

| .NET Version      |        Supported       |         Until        |
|-------------------|:----------------------:|:--------------------:|
| .NET 6.0          | :heavy_check_mark: Yes | :bangbang: Dez. 2024 |
| .NET 7.0          | :heavy_check_mark: Yes |                      |
| .NET 8.0          | :heavy_check_mark: Yes |                      |

Why have we decided to this approach? Because we want to be able to use the latest features of the .NET platform,
and the performance gains that come with it. We know that not every of our NuGet packages will gain performance
from this, but this is our general strategy and nobody knows what the future will bring.