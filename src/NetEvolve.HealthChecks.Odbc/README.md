# NetEvolve.HealthChecks.Odbc

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Odbc?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Odbc/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Odbc?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Odbc/)

This package provides a health check for all ODBC data sources, based on the [System.Data.Odbc](https://www.nuget.org/packages/System.Data.Odbc/) package.
The main purpose is to check if the ODBC data source is available and if the database is online.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Odbc
```

## Health Check - ODBC Liveness
The health check is a liveness check. It checks if the ODBC data source is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Odbc;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `odbc` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple ODBC data sources to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddOdbc("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Odbc": {
      "<name>": {
        "ConnectionString": "<connection string>",
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you have only one ODBC data source to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddOdbc("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddOdbc("<name>", options => ..., "sqlserver", "database");
```