# NetEvolve.HealthChecks

![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks?logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks?logo=nuget)

This package contains additional health checks for the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks/) package.
Primarily for testing the healthiness of the own application, like application service is started or stopped.

:bulb: This package is available for .NET 6.0 and later.

## Installation
To use this package, you need to add the package to your project.You can do this by using the NuGet package manager or by using the dotnet CLI.

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

:bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();
    builder.AddApplicationHealthy("ping", "pong", ...);
```

## Health Check - Application Ready
This health check is used to check the readiness of the application.
It will return `Unhealthy` until the `IHostApplicationLifetime` triggers the `ApplicationStarted` event, after that it will return `Healthy`.
Until the `IHostApplicationLifetime` triggers the `ApplicationStopping` event, after that it will return `Unhealthy`.

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