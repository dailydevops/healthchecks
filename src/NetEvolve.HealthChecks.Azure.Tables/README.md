# NetEvolve.HealthChecks.Azure.Tables

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Tables?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Tables?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/)

This package provides a health check for Azure Tables, based on the [Azure.Storage.Tables](https://www.nuget.org/packages/Azure.Storage.Tables/) package. The main purpose is to check that the Azure Table Service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later
- Active Azure subscription
- Azure Storage Account created
- Table created (or permissions to create tables)
- Valid connection string or managed identity configured
- Network connectivity to Azure Storage endpoints

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Tables
```

## Health Check - Azure Table Client Availability
The health check is a liveness check. It will check that the Azure Table Service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Tables` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Tables;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `blob` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureTableClient` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureTableClint("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureTableClient": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required
        "ContainerName": "<container-name>", // required
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `AzureTableClientOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureTableClient("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.ContainerName = "<container-name>";
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAzureTableClient("<name>", options => ..., "azure");
```