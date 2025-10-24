# NetEvolve.HealthChecks.Consul

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Consul?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Consul/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Consul?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Consul/)

This package provides health checks for HashiCorp Consul, based on the `Consul` NuGet package.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Consul
```

## Health Check - Consul Liveness
The health check is a liveness check. It verifies connectivity to the Consul server and the ability to query the Consul leader status.
If the operation needs longer than the configured timeout, the health check will return `Degraded`.
If the server is unreachable or returns an invalid response, the health check will return `Unhealthy`.

## Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Consul;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `consul` and `service-discovery` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple Consul instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddConsul("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Consul": {
      "<name>": {
        "Timeout": 1000 // Optional, timeout in milliseconds, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one Consul instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddConsul("<name>", options =>
{
    options.Timeout = 1000; // Optional, timeout in milliseconds, default is 100 milliseconds
    options.KeyedService = "<key>"; // Optional, used when multiple Consul clients are registered
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddConsul("<name>", options => ..., "consul", "service-discovery");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
