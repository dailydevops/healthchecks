# NetEvolve.HealthChecks.Azure.Files

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Files?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Files/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Files?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Files/)

This package provides health checks for Azure File Storage, to check the availability of file shares and the file service itself.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Files
```

## Health Check - Azure File Share Availability
The health check is a liveness check. It will check that the Azure File Service and the File Share is reachable and that the client can connect to it. If the service or the share needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service or the share is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Files` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Files;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `files` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureFileShare` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddFileShareAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureFileShare": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required
        "ShareName": "<share-name>", // required
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `FileShareAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddFileShareAvailability("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.ShareName = "<share-name>";
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddFileShareAvailability("<name>", options => ..., "azure");
```

## Health Check - Azure File Service Availability
The health check is a liveness check. It will check that the Azure File Service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Files` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Files;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `files` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureFileService` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddFileServiceAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureFileService": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `FileServiceAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddFileServiceAvailability("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddFileServiceAvailability("<name>", options => ..., "azure");
```