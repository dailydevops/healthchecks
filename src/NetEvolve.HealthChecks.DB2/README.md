# NetEvolve.HealthChecks.DB2

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.DB2?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.DB2?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2/)

This package provides a health check for IBM's DB2 databases, based on the [Net.IBM.Data.Db2 (Windows)](https://www.nuget.org/packages/Net.IBM.Data.Db2/), [Net.IBM.Data.Db2-lnx (Linux)](https://www.nuget.org/packages/Net.IBM.Data.Db2-lnx/) and [Net.IBM.Data.Db2-osx (OSX)](https://www.nuget.org/packages/Net.IBM.Data.Db2-osx/) packages. The main purpose is to check if the database is available and if the database is online.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.DB2
```

> **Warning**
This package has no direct reference on the `Net.IBM.Data.Db2` package. You have to add the package manually to your project, if you want to use the health check. Based on your operating system, you have to add one of the following packages:
- [Net.IBM.Data.Db2 (Windows)](https://www.nuget.org/packages/Net.IBM.Data.Db2/)
- [Net.IBM.Data.Db2-lnx (Linux)](https://www.nuget.org/packages/Net.IBM.Data.Db2-lnx/)
- [Net.IBM.Data.Db2-osx (OSX)](https://www.nuget.org/packages/Net.IBM.Data.Db2-osx/)

## Health Check - IBM DB2 Liveness
The health check is a liveness check. It checks if the DB2 Server is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.DB2;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

:heavy_exclamation_mark: The configuration of this package is compatible with the [NetEvolve.HealthChecks.DB2.Connector](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2.Connector/) package.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `db2` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple DB2 instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddDB2("<name>");
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

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one SQL Server instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddDB2("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddDB2("<name>", options => ..., "DB2", "database");
```

## Related Packages

### See Also
- <a>`NetEvolve.HealthChecks.Abstractions`</a> - Base abstractions for creating custom health checks