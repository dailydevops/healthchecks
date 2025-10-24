# NetEvolve.HealthChecks

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks/)

This package contains additional health checks for the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks/) package.
Primarily for testing the healthiness of the own application, like application service is started or stopped.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later
- ASP.NET Core application

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.

```powershell
dotnet add package NetEvolve.HealthChecks
```

## Health Check - Application Healthy
This health check is providing always a `Healthy` status. It can be used to check if the application is running, like a ping.

### Usage
This healthcheck does not require any configuration, it will automatically registered as `ApplicationHealthy` health check.

```csharp
var builder = services.AddHealthChecks();
builder.AddApplicationHealthy();
```

## Health Check - Application Ready
This health check is used to check the readiness of the application. It returns `Unhealthy` until the `IHostApplicationLifetime` triggers the `ApplicationStarted` event, then it returns `Healthy`. It returns `Healthy` until the `IHostApplicationLifetim`e triggers the `ApplicationStopping` event, after which it returns `Unhealthy`.

### Usage
This healthcheck does not require any configuration, it will automatically registered as `ApplicationReady` health check.

```csharp
var builder = services.AddHealthChecks();
builder.AddApplicationReady()
```


### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddApplicationReady("readiness");
```