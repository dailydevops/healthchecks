# NetEvolve.HealthChecks.Azure.DigitalTwin

[![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.DigitalTwin?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.DigitalTwin/)
[![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.DigitalTwin?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.DigitalTwin/)

This package provides health check implementations for Azure Digital Twins.

:bulb: This package is available for .NET 6.0 and later.

## Installation

To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.

```powershell
dotnet add package NetEvolve.HealthChecks.Azure.DigitalTwin
```

## Health Check - Azure Digital Twins Service Availability

The health check is a liveness check. It will check that the Azure Digital Twins service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage

After adding the package, you need to import the namespace and add the health check to the health check builder.

```csharp
using NetEvolve.HealthChecks.Azure.DigitalTwin;
```

Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters

- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `digitaltwins` and `iot` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based

The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureDigitalTwins` to your `appsettings.json` file.

```csharp
var builder = services.AddHealthChecks();

builder.AddDigitalTwinServiceAvailability("<name>");
```

The configuration looks like this:

```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureDigitalTwins": {
      "<name>": {
        "ServiceUri": "<service-uri>", // required
        "Mode": "DefaultAzureCredentials", // optional, default is ServiceProvider
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based

The second one is to use the options based approach. Therefore, you have to create an instance of `DigitalTwinServiceAvailableOptions` and provide the configuration.

```csharp
var builder = services.AddHealthChecks();

builder.AddDigitalTwinServiceAvailability("<name>", options =>
{
    options.ServiceUri = new Uri("<service-uri>");
    options.Mode = DigitalTwinClientCreationMode.DefaultAzureCredentials;
    options.Timeout = 100;
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddDigitalTwinServiceAvailability("<name>", options => ..., "azure", "digitaltwins");
```

## Configuration Modes

### ServiceProvider Mode

When using the `ServiceProvider` mode, the health check will use the pre-registered `DigitalTwinsClient` from the service provider. This is the default mode and requires you to register the client manually.

```csharp
services.AddAzureClients(clients => 
{
    clients.AddDigitalTwinsClient(new Uri("<service-uri>"));
});
```

### DefaultAzureCredentials Mode

When using the `DefaultAzureCredentials` mode, the health check will create a new `DigitalTwinsClient` using the `DefaultAzureCredential` and the provided service URI.

This mode is useful when you want to use managed identity or other Azure credential types supported by the `DefaultAzureCredential` chain.