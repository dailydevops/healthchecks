# NetEvolve.HealthChecks.Dapr

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Dapr?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Dapr/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Dapr?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Dapr/)

This package provides a health check for Dapr, based on the [Dapr.Client](https://www.nuget.org/packages/Dapr.Client/) package. The main purpose is to check if the Dapr sidecar is available and responding to requests.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Dapr
```

## Health Check - Dapr Liveness
The health check is a liveness check. It checks if the Dapr sidecar is available and responding to requests.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Dapr;
```
Therefore, you can use two different approaches.

### Parameters
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tag `dapr` is always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you want to configure the health check through your application settings.
```csharp
var builder = services.AddHealthChecks();

builder.AddDapr();
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "DaprSidecar": {
      "Timeout": "<timeout>" // optional, default is 100 milliseconds
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you want to configure the health check programmatically with dynamic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddDapr(options =>
{
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddDapr(options => ..., "dapr", "sidecar");
```


## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
