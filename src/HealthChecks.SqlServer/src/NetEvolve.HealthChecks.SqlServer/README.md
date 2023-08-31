# NetEvolve.HealthChecks.SqlServer

![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SqlServer?logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlServer?logo=nuget)

This package provides a health check for SQL Server, based on the [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) package.
The main purpose is to check if the SQL Server is available and if the database is online.

:bulb: This package is available for .NET 6.0 and later.

## Installation
To use this package, you need to add the package to your project.You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.SqlServer
```

## Health Check - SqlServerLegacy Liveness
The health check is a liveness check. It checks if the SQL Server is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.SqlServer;
```
Therefor you can use two different approaches. In both approaches you have to provide a name for the health check.

:heavy_exclamation_mark: Please be aware that the name is prefixed by "SqlServer" in the health check. The prefixed Name will be used to identify the configuration object.

**Examples:**

| Name            | Prefixed Name          |
| --------------- | ---------------------- |
| `MySqlServer`   | `SqlServerMySqlServer` |
| `1`             | `SqlServer1`           |
| `SqlServerTest` | `SqlServerTest`        |

:heavy_exclamation_mark: The configuration of this package is compatible with the [NetEvolve.HealthChecks.SqlServer.Legacy](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy/) package.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple SQL Server instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddSqlServer("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "SqlServer<name>": {
      "ConnectionString": "<connection string>",
      "Timeout": "<timeout>" // optional, default is 100 milliseconds
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you have only one SQL Server instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddSqlServer("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();
    builder.AddSqlServer("<name>", options => ..., "sqlserver", "database");
```