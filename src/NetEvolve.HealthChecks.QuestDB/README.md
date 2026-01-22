# NetEvolve.HealthChecks.QuestDB

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.QuestDB?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.QuestDB/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.QuestDB?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.QuestDB/)

This package provides a health check for QuestDB databases, based on the [net-questdb-client](https://www.nuget.org/packages/net-questdb-client/) package.
The main purpose is to check if the QuestDB database is available and if the database is online.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.QuestDB
```

## Health Check - QuestDB Liveness
The health check is a liveness check. It checks if the QuestDB database is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.QuestDB` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.QuestDB;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `questdb` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration-based
The first one is to use the configuration-based approach. This approach is recommended if you have multiple QuestDB instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddQuestDB("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "QuestDB": {
      "<name>": {
        "StatusUri": "<http-api-uri>", // uri to the db api to run the status request against
        "Timeout": <timeout> // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder-based
The second approach is to use the builder-based approach. This approach is recommended if you only have one QuestDB instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddQuestDB("<name>", options =>
{
    options.StatusUri = "<http-api-uri>"; // uri to the db api to run the status request against
    options.Timeout = <timeout>; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddQuestDB("<name>", options => ..., "questdb", "database");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
