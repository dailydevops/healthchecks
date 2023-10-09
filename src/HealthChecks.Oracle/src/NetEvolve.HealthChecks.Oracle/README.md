# NetEvolve.HealthChecks.Oracle

![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Oracle?logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Oracle?logo=nuget)

This package provides a health check for Oracle databases, based on the [Oracle.ManagedDataAccess.Core](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/) package.
The main purpose is to check if the Oracle database is available and if the database is online.

:bulb: This package is available for .NET 6.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Oracle
```

## Health Check - Oracle Liveness
The health check is a liveness check. It checks if the Oracle database is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Oracle` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Oracle;
```
Therefor you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The Name will be used to identify the configuration object.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `oracle` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple SQL Server instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddOracle("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Oracle": {
      "<name>": {
        "ConnectionString": "<connection string>",
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you have only one SQL Server instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddOracle("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();
    builder.AddOracle("<name>", options => ..., "oracle", "database");
```