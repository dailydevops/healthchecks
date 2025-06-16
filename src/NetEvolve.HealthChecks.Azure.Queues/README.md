# NetEvolve.HealthChecks.Azure.Queues

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Queues?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Queues?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/)

This package provides a health check for Azure Queues, based on the [Azure.Storage.Queues](https://www.nuget.org/packages/Azure.Storage.Queues/) package. The main purpose is to check that the Azure Queue Service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Queues
```

## Health Check - Azure Queue Client Availability
The health check is a liveness check. It will check that the Azure Queue Service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, yo need to import the namespace `NetEvolve.HealthChecks.Azure.Queues` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Queues;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `queue` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureQueueClient` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureQueueClint("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureQueueClient": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required
        "QueueName": "<queue-name>", // required
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `AzureQueueClientOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureQueueClient("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.QueueName = "<queue-name>";
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAzureQueueClient("<name>", options => ..., "azure");
```