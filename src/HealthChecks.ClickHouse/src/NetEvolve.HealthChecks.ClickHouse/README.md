# NetEvolve.HealthChecks.ClickHouse

[![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.ClickHouse?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse/)
[![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.ClickHouse?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse/)

This package provides a health check for ClickHouse, based on the [ClickHouse.Client](https://www.nuget.org/packages/ClickHouse.Client/) package.
The main purpose is to check if the ClickHouse is available and if the database is online.

:bulb: This package is available for .NET 6.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.ClickHouse
```

## Health Check - ClickHouseLegacy Liveness
The health check is a liveness check. It checks if the ClickHouse is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.ClickHouse;
```
Therefor you can use two different approaches. In both approaches you have to provide a name for the health check.

:heavy_exclamation_mark: The configuration of this package is compatible with the [NetEvolve.HealthChecks.ClickHouse.Legacy](https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse.Legacy/) package.

### Parameters
- `name`: The name of the health check. The name will be used to identify the configuration object.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `ClickHouse` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple ClickHouse instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddClickHouse("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "ClickHouse": {
      "<name>": {
        "ConnectionString": "<connection string>",
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you have only one ClickHouse instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddClickHouse("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();
    builder.AddClickHouse("<name>", options => ..., "clickhouse", "database");
```