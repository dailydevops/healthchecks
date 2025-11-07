# NetEvolve.HealthChecks.Azure.EventHubs

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.EventHubs?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.EventHubs/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.EventHubs?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.EventHubs/)

This package provides a health check for Azure Event Hubs, based on the [Azure.Messaging.EventHubs](https://www.nuget.org/packages/Azure.Messaging.EventHubs/) package.
The main purpose is to check that the Azure Event Hubs namespace is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.EventHubs
```

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.EventHubs` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.EventHubs;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

## Health Check - Azure Event Hubs

The health check is a readiness check. It will check that the Azure Event Hubs namespace is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `eventhubs` and `messaging` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureEventHubs` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureEventHubs("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureEventHubs": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required when Mode is ConnectionString
        "Mode": "<mode>", // required, to specify the client creation mode, either `ServiceProvider`, `DefaultAzureCredentials` or `ConnectionString`
        "EventHubName": "<event-hub-name>", // required
        "FullyQualifiedNamespace": "<fully-qualified-namespace>", // required when Mode is DefaultAzureCredentials
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `EventHubsOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureEventHubs("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.Mode = ClientCreationMode.ServiceProvider; // or DefaultAzureCredentials or ConnectionString
    options.EventHubName = "<event-hub-name>";
    options.Timeout = TimeSpan.FromMilliseconds(100); // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAzureEventHubs("<name>", options => ..., "azure-eventhubs");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
