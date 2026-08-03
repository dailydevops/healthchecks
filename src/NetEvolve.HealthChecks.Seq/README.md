# NetEvolve.HealthChecks.Seq

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Seq?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Seq/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Seq?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Seq/)

This package provides a health check for the [Seq](https://datalust.co/seq) logging server, based on the [Seq.Api](https://www.nuget.org/packages/Seq.Api/) package. The main purpose is to check if the Seq server is available.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Seq
```

## Health Check - Seq Liveness
The health check is a liveness check. It checks if the Seq server is reachable and available.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Seq;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `seq` and `logging` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple Seq instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddSeq("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Seq": {
      "<name>": {
        "Mode": "<client_creation_mode>", // Optional, defaults to 'SeqClientCreationMode.ServiceProvider'
        "KeyedService": "<key>", // Optional, used when Mode set to 'SeqClientCreationMode.ServiceProvider'
        "ServerUrl": "<server_url>", // Required when Mode set to 'SeqClientCreationMode.ServerUrl'
        "ApiKey": "<api_key>", // Optional, used when Mode set to 'SeqClientCreationMode.ServerUrl'
        "Timeout": "<timeout>" // Optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one server instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddSeq("<name>", options =>
{
    options.Mode = <client_creation_mode>; // Optional, defaults to 'SeqClientCreationMode.ServiceProvider'
    options.KeyedService = "<key>"; // Optional, used when Mode set to 'SeqClientCreationMode.ServiceProvider'
    options.ServerUrl = "<server_url>"; // Required when Mode set to 'SeqClientCreationMode.ServerUrl'
    options.ApiKey = "<api_key>"; // Optional, used when Mode set to 'SeqClientCreationMode.ServerUrl'
    options.Timeout = <timeout>; // Optional, defaults to 100 milliseconds

    // Optional, defaults to NetEvolve.HealthChecks.Seq.SeqHealthCheck.DefaultCommandAsync
    options.CommandAsync = async (connection, cancellationToken) => {
        // Your custom server pinging logic goes here.
        // Should return true if the command result is valid, false otherwise.
    };
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddSeq("<name>", options => ..., "Seq", "observability");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
