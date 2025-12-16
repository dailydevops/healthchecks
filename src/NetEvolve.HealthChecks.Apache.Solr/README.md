# NetEvolve.HealthChecks.Apache.Solr

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Apache.Solr?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Solr/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Apache.Solr?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Solr/)

This package provides a health check for Apache Solr, based on the [SolrNet.Core](https://www.nuget.org/packages/SolrNet.Core/) package.
The main purpose is to check that the Solr server is reachable. The health check uses the Solr admin ping endpoint and does not require a core/collection name.

:bulb: This package targets .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Apache.Solr
```

## Health Check - Apache Solr Liveness
This health check verifies Solr reachability via the admin ping endpoint.
- If the server responds slower than the configured timeout, the result is `Degraded`.
- If the server is not reachable or returns an error, the result is `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Apache.Solr` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Apache.Solr;
```
You can register the health check via configuration or code. In both approaches you must provide a unique name.

### Parameters
- `name`: Unique health check name used to match configuration.
- `options`: Optional configuration when using the builder approach. Supported options: `BaseUrl` (string), `Timeout` (milliseconds), and `CreationMode` (`Create` or `ServiceProvider`).
- `tags`: Optional tags appended to the defaults. The tags `solr`, `apache`, and `search` are always included.

### Variant 1: Configuration based
Add a configuration section under `HealthChecks:Solr` in `appsettings.json`, then register by name.
```csharp
var builder = services.AddHealthChecks();

builder.AddSolr("<name>");
```

Example `appsettings.json`:
```json
{
  "HealthChecks": {
    "Solr": {
      "my-solr": {
        "BaseUrl": "http://localhost:8983",
        "Timeout": 100,
        "CreationMode": "Create" // or "ServiceProvider" when Solr client is registered in DI
      }
    }
  }
}
```

### Variant 2: Builder based
Use this approach when values are dynamic or you have a single instance.
```csharp
var builder = services.AddHealthChecks();

builder.AddSolr("my-solr", options =>
{
  options.BaseUrl = "http://localhost:8983";
  options.Timeout = 100; // milliseconds
  options.CreationMode = ClientCreationMode.Create; // default: ServiceProvider
});
```

Example `Program.cs` registration and endpoint exposure:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
  .AddSolr("my-solr");

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
```

When using `CreationMode = ServiceProvider`, ensure an `ISolrBasicReadOnlyOperations<string>` client is registered in DI before adding the health check.

### Tags
You can provide additional tags to group or filter health checks.

```csharp
var builder = services.AddHealthChecks();

builder.AddSolr("my-solr", options => { /* ... */ }, "critical", "liveness");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
