# NetEvolve.HealthChecks.Meilisearch

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Meilisearch?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Meilisearch/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Meilisearch?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Meilisearch/)

This package provides a health check for Meilisearch instances, based on the [MeiliSearch](https://www.nuget.org/packages/MeiliSearch/) package. The main purpose is to check if the server is available and healthy.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Meilisearch
```

## Health Check - Meilisearch Liveness
The health check is a liveness check. It checks if the Meilisearch instance is available and healthy.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Meilisearch;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `meilisearch` and `search` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple Meilisearch instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddMeilisearch("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Meilisearch": {
      "<name>": {
        "Mode": "<client_creation_mode>", // Optional, defaults to 'MeilisearchClientCreationMode.ServiceProvider'
        "KeyedService": "<key>", // Optional, used when Mode set to 'MeilisearchClientCreationMode.ServiceProvider'
        "Host": "<host_url>", // Required when Mode set to 'MeilisearchClientCreationMode.Internal'
        "ApiKey": "<api_key>", // Optional, used when Mode set to 'MeilisearchClientCreationMode.Internal'
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

builder.AddMeilisearch("<name>", options =>
{
    options.Mode = <client_creation_mode>; // Optional, defaults to 'MeilisearchClientCreationMode.ServiceProvider'
    options.KeyedService = "<key>"; // Optional, used when Mode set to 'MeilisearchClientCreationMode.ServiceProvider'
    options.Host = "<host_url>"; // Required when Mode set to 'MeilisearchClientCreationMode.Internal'
    options.ApiKey = "<api_key>"; // Optional, used when Mode set to 'MeilisearchClientCreationMode.Internal'
    options.Timeout = <timeout>; // Optional, defaults to 100 milliseconds

    // Optional, defaults to NetEvolve.HealthChecks.Meilisearch.DefaultCommandAsync
    options.CommandAsync = async (client, cancellationToken) =>
    {
        // Your custom health check logic here.
        // Should return true if the health check is successful, false otherwise.
    };
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddMeilisearch("<name>", options => ..., "Meilisearch", "search");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
