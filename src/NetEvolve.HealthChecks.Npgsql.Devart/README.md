# NetEvolve.HealthChecks.Npgsql.Devart

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Npgsql.Devart?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql.Devart/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Npgsql.Devart?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql.Devart/)

This package provides a health check for PostgreSQL, based on the [Devart.Data.PostgreSql](https://www.nuget.org/packages/Devart.Data.PostgreSql/) package.
The main purpose is to check if the PostgreSQL database is available and if the database is online.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Npgsql.Devart
```

## Health Check - NpgsqlDevart Liveness
The health check is a liveness check. It checks if the PostgreSQL database is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Npgsql.Devart;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

:heavy_exclamation_mark: The configuration of this package is compatible with the [NetEvolve.HealthChecks.Npgsql](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql/) package. If you want to use the new package, you can simply replace the package and the configuration will be compatible.

### Parameters
- `name`: The name of the health check.
- `options`: The configuration options for the health check. Optional.
- `tags`: The tags for the health check. Optional.
  - `postgresql`: This is always used as tag, so that you can filter for all PostgreSQL health checks.
  - `database`: This is always used as tag, so that you can filter for all database health checks.
  - `devart`: This is always used as tag, so that you can filter for all Devart health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple PostgreSQL instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddPostgreSqlDevart("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "PostgreSql": {
      "<name>": {
        "ConnectionString": "<connection string>",
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one PostgreSQL instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddPostgreSqlDevart("<name>", options =>
{
    options.ConnectionString = "<connection string>";
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddPostgreSqlDevart("<name>", options => ..., "postgresql", "database");
```