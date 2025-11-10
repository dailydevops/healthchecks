# NetEvolve.HealthChecks.EventStoreDb

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.EventStoreDb?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.EventStoreDb/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.EventStoreDb?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.EventStoreDb/)

This package provides a health check for EventStoreDb, based on the [EventStore.Client.Grpc.Streams](https://www.nuget.org/packages/EventStore.Client.Grpc.Streams/) package. The main purpose is to check if the EventStoreDb is available and if the connection is healthy.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.EventStoreDb
```

## Health Check - EventStoreDb Liveness
The health check is a liveness check. It checks if the EventStoreDb is available and if the connection is healthy.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.EventStoreDb;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `eventstoredb` and `nosql` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple EventStoreDb instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddEventStoreDb("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "EventStoreDb": {
      "<name>": {
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one EventStoreDb instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddEventStoreDb("<name>", options =>
{
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddEventStoreDb("<name>", options => ..., "EventStoreDb", "database");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
