# NetEvolve.HealthChecks.Azure.Blobs

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Blobs?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Blobs?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/)

This package provides a health check for Azure Blobs, based on the [Azure.Storage.Blobs](https://www.nuget.org/packages/Azure.Storage.Blobs/) package. The main purpose is to check that the Azure Blob Service and the Storage Container is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Blobs
```

## Health Check - Azure Blob Container Availability
The health check is a liveness check. It will check that the Azure Blob Service and the Storage Container is reachable and that the client can connect to it. If the service or the container needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service or the container is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Blobs` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Blobs;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `blob` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureBlobContainer` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureBlobContainer("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureBlobContainer": {
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
The second one is to use the options based approach. Therefore, you have to create an instance of `AzureBlobContainerOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureBlobContainer("<name>", options =>
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

builder.AddAzureBlobContainer("<name>", options => ..., "azure");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
