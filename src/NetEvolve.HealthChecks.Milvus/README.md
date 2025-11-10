# NetEvolve.HealthChecks.Milvus

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Milvus?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Milvus/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Milvus?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Milvus/)

This package provides a health check for Milvus, based on the [Milvus.Client](https://www.nuget.org/packages/Milvus.Client/) package. The main purpose is to check that the Milvus service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Milvus
```

## Health Check - Milvus Service Availability
The health check is a liveness check. It will check that the Milvus service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Milvus` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Milvus;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `milvus` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Milvus` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddMilvus("<name>");
```

The configuration looks like this:
```json
{
    ..., // other configuration
    "HealthChecks": {
        "Milvus": {
            "<name>": {
                "KeyedService": "<name>", // optional, must be given if you want to access a keyed service
                ..., // other configuration
                "Timeout": "<timeout>" // optional, default is 100 milliseconds
            }
        }
    }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `MilvusOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddMilvus("<name>", options =>
{
        options.KeyedService = "<name>"; // optional, must be given if you want to access a keyed service
        ...
        options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddMilvus("<name>", options => ..., "milvus");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
