# NetEvolve.HealthChecks.Azure.Search

[![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Search?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Search/)
[![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Search?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Search/)

This package provides a health check for Azure Cognitive Search, to check the availability of the search service.

:bulb: This package is available for .NET 6.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Search
```

## Health Check - Azure Cognitive Search Service Availability
The health check is a liveness check. It will check that the Azure Cognitive Search Service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, yo need to import the namespace `NetEvolve.HealthChecks.Azure.Search` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Search;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `search` and `cognitive` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureSearchService` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddSearchServiceAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureSearchService": {
      "<name>": {
        "ServiceUri": "https://<search-service-name>.search.windows.net", // required
        "ApiKey": "<api-key>", // required when using ApiKey mode
        "Mode": "ApiKey", // required, allowed values: ServiceProvider, ConnectionString, DefaultAzureCredentials, ApiKey
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `SearchServiceAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddSearchServiceAvailability("<name>", options =>
{
    options.ServiceUri = new Uri("https://<search-service-name>.search.windows.net");
    options.ApiKey = "<api-key>";
    options.Mode = SearchClientCreationMode.ApiKey;
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddSearchServiceAvailability("<name>", options => ..., "azure");
```

## Health Check - Azure Cognitive Search Index Availability
The health check is a liveness check. It will check that a specific Azure Cognitive Search Index is reachable and that the client can connect to it. If the service or the index needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service or the index is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, yo need to import the namespace `NetEvolve.HealthChecks.Azure.Search` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Search;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `search` and `cognitive` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureSearchIndex` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddSearchIndexAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureSearchIndex": {
      "<name>": {
        "ServiceUri": "https://<search-service-name>.search.windows.net", // required
        "ApiKey": "<api-key>", // required when using ApiKey mode
        "IndexName": "<index-name>", // required
        "Mode": "ApiKey", // required, allowed values: ServiceProvider, ConnectionString, DefaultAzureCredentials, ApiKey
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `SearchIndexAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddSearchIndexAvailability("<name>", options =>
{
    options.ServiceUri = new Uri("https://<search-service-name>.search.windows.net");
    options.ApiKey = "<api-key>";
    options.IndexName = "<index-name>";
    options.Mode = SearchClientCreationMode.ApiKey;
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddSearchIndexAvailability("<name>", options => ..., "azure");
```