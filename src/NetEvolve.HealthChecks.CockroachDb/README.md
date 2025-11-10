# NetEvolve.HealthChecks.CockroachDb

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.CockroachDb?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.CockroachDb/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.CockroachDb?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.CockroachDb/)

This package provides a health check for CockroachDb databases, based on the [Npgsql](https://www.nuget.org/packages/Npgsql/) package.
The main purpose is to check if the CockroachDb database is available and if the database is online.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.CockroachDb
```

## Health Check - CockroachDb Liveness
The health check is a liveness check. It checks if the CockroachDb database is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.CockroachDb` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.CockroachDb;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `cockroachdb` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple CockroachDb instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddCockroachDb("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "CockroachDb": {
      "<name>": {
        "ConnectionString": "<connection string>",
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one CockroachDb instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddCockroachDb("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddCockroachDb("<name>", options => ..., "cockroachdb", "database");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
