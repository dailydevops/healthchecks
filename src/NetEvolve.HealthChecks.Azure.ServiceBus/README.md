# NetEvolve.HealthChecks.Azure.ServiceBus

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.ServiceBus?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ServiceBus/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.ServiceBus?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ServiceBus/)

This package provides a health check for Azure Service Bus, based on the [Azure.Messaging.ServiceBus](https://www.nuget.org/packages/Azure.Messaging.ServiceBus/) package.
The main purpose is to check that the Azure Service Bus namespace is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.ServiceBus
```

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.ServiceBus` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.ServiceBus;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

## Health Check - Azure ServiceBus Queue

The health check is a readiness check. It will check that the Azure ServiceBus queue is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `servicebus` and `messaging` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureServiceBusQueue` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureServiceBusQueue("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureServiceBusQueue": {
      "<name>": {
        "KeyedService": "<key>", // optional, must be given if you want to access a keyed service
        "ConnectionString": "<connection-string>", // required
        "Mode": "<mode>", // required, to specify the client creation mode, either `ServiceProvider`, `DefaultAzureCredentials` or `ConnectionString`
        "QueueName": "<queue-name>", // required
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `AzureServiceBusQueueOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureServiceBusQueue("<name>", options =>
{
    options.KeyedService = "<key>"; // optional, must be given if you want to access a keyed service
    options.ConnectionString = "<connection-string>";
    options.Mode = ClientCreationMode.ServiceProvider; // or DefaultAzureCredentials or ConnectionString
    options.QueueName = "<queue-name>";
    options.Timeout = TimeSpan.FromMilliseconds(100); // optional, default is 100 milliseconds
});
```

## Health Check - Azure ServiceBus Subscription
The health check is a readiness check. It will check that the Azure ServiceBus subscription is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `servicebus` and `messaging` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based

The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureServiceBusSubscription` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureServiceBusSubscription("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureServiceBusSubscription": {
      "<name>": {
        "KeyedService": "<key>", // optional, must be given if you want to access a keyed service
        "ConnectionString": "<connection-string>", // required
        "Mode": "<mode>", // required, to specify the client creation mode, either `ServiceProvider`, `DefaultAzureCredentials` or `ConnectionString`
        "TopicName": "<topic-name>", // required
        "SubscriptionName": "<subscription-name>", // required
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based

The second one is to use the options based approach. Therefore, you have to create an instance of `AzureServiceBusSubscriptionOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureServiceBusSubscription("<name>", options =>
{
    options.KeyedService = "<key>"; // optional, must be given if you want to access a keyed service
    options.ConnectionString = "<connection-string>";
    options.Mode = ClientCreationMode.ServiceProvider; // or DefaultAzureCredentials or ConnectionString
    options.TopicName = "<topic-name>";
    options.SubscriptionName = "<subscription-name>";
    options.Timeout = TimeSpan.FromMilliseconds(100); // optional, default is 100 milliseconds
});
```

## Health Check - Azure ServiceBus Topic
The health check is a readiness check. It will check that the Azure ServiceBus topic is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `servicebus` and `messaging` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based

The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureServiceBusTopic` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureServiceBusTopic("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureServiceBusTopic": {
      "<name>": {
        "KeyedService": "<key>", // optional, must be given if you want to access a keyed service
        "ConnectionString": "<connection-string>", // required
        "Mode": "<mode>", // required, to specify the client creation mode, either `ServiceProvider`, `DefaultAzureCredentials` or `ConnectionString`
        "TopicName": "<topic-name>", // required
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based

The second one is to use the options based approach. Therefore, you have to create an instance of `AzureServiceBusTopicOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureServiceBusTopic("<name>", options =>
{
    options.KeyedService = "<key>"; // optional, must be given if you want to access a keyed service
    options.ConnectionString = "<connection-string>";
    options.Mode = ClientCreationMode.ServiceProvider; // or DefaultAzureCredentials or ConnectionString
    options.TopicName = "<topic-name>";
    options.Timeout = TimeSpan.FromMilliseconds(100); // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAzureServiceBus("<name>", options => ..., "azure-servicebus");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
