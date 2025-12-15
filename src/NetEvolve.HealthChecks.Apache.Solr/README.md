# NetEvolve.HealthChecks.Apache.Solr

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Apache.Solr?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Solr/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Apache.Solr?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Solr/)

This package provides a health check for Apache Solr, based on the [SolrNet.Core](https://www.nuget.org/packages/SolrNet.Core/) package.
The main purpose is to check that the Solr server is reachable and that the specified core is available.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Apache.Solr
```

## Health Check - Apache Solr Liveness
The health check is a liveness check. It will check that the Solr server is reachable and that the specified core is available using the admin ping endpoint.
If the server needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the server is not reachable or returns an error status, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Apache.Solr` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Apache.Solr;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `solr`, `apache` and `search` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Solr` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddSolr("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Solr": {
      "<name>": {
        "BaseUrl": "<solr-base-url>", // required, e.g. http://localhost:8983
        "Core": "<core-name>", // required, e.g. collection1
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one Solr instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddSolr("<name>", options =>
{
    options.BaseUrl = new Uri("<solr-base-url>"); // required, e.g. http://localhost:8983
    options.Core = "<core-name>"; // required, e.g. collection1
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddSolr("<name>", options => ..., "solr", "search");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
