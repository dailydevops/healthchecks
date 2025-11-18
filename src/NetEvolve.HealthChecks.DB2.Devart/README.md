# NetEvolve.HealthChecks.DB2.Devart

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.DB2.Devart?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2.Devart/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.DB2.Devart?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2.Devart/)

This package provides a health check for IBM DB2, based on the [Devart.Data.DB2](https://www.nuget.org/packages/Devart.Data.DB2/) package.
The main purpose is to check if the DB2 database is available and if the database is online.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.DB2.Devart
```

## Health Check - DB2Devart Liveness
The health check is a liveness check. It checks if the DB2 database is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.DB2.Devart;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

:heavy_exclamation_mark: The configuration of this package is compatible with the [NetEvolve.HealthChecks.DB2](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2/) package. If you want to use the new package, you can simply replace the package and the configuration will be compatible.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `db2`, `database` and `devart` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

#### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple DB2 instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddDB2Devart("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "DB2": {
      "<name>": {
        "ConnectionString": "<connection string>",
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

#### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you have only one DB2 instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddDB2Devart("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddDB2Devart("<name>", options => ..., "db2", "database");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
