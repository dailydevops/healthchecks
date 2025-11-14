# NetEvolve.HealthChecks.Azure.Tables

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Tables?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Tables?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/)

This package provides a health check for Azure Tables, based on the [Azure.Storage.Tables](https://www.nuget.org/packages/Azure.Storage.Tables/) package. The main purpose is to check that the Azure Table Service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Tables
```

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Tables` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Tables;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.


## Health Check - Azure Table Client Availability
The health check is a liveness check. It will check that the Azure Table client is reachable. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `table` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureTableClient` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddTableClientAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureTableClient": {
      "<name>": {
        "KeyedService": "<service-key>",           // optional, used when multiple clients are registered and `Mode` is `ServiceProvider`
        "ConnectionString": "<connection-string>", // required when `Mode` is `ConnectionString`
        "Mode": "<client-creation-mode>",          // required to specify the client creation mode
        "ServiceUri": "<service-uri>",             // required when `Mode` is `DefaultAzureCredentials`, `SharedKey` or `AzureSasCredential`
        "TableName": "<table-name>",               // required
        "AccountName": "<account-name>",           // required when `Mode` is `SharedKey`
        "AccountKey": "<account-key>",             // required when `Mode` is `SharedKey`
        "Timeout": "<timeout>"                     // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `TableClientAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddTableClientAvailability("<name>", options =>
{
    options.KeyedService = "<service-key>";           // optional, used when multiple clients are registered
    options.ConnectionString = "<connection-string>"; // required when `Mode` is `ConnectionString`
    options.Mode = "<client-creation-mode>";          // required to specify the client creation mode
    options.ServiceUri = "<service-uri>",             // required when `Mode` is `DefaultAzureCredentials`, `SharedKey` or `AzureSasCredential`
    options.TableName = "<table-name>";               // required
    options.AccountName = "<account-name>",           // required when `Mode` is `SharedKey`
    options.AccountKey = "<account-key>",             // required when `Mode` is `SharedKey`
    options.Timeout = "<timeout>"                     // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddTableClientAvailability("<name>", options => ..., "azure");
```


## Health Check - Azure Table Service Availability
The health check is a liveness check. It will check that the Azure Table service is reachable. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `table` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureTableService` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddTableServiceAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureTableService": {
      "<name>": {
        "KeyedService": "<service-key>",           // optional, used when multiple clients are registered and `Mode` is `ServiceProvider`
        "ConnectionString": "<connection-string>", // required when `Mode` is `ConnectionString`
        "Mode": "<client-creation-mode>",          // required to specify the client creation mode
        "ServiceUri": "<service-uri>",             // required when `Mode` is `DefaultAzureCredentials`, `SharedKey` or `AzureSasCredential`
        "AccountName": "<account-name>",           // required when `Mode` is `SharedKey`
        "AccountKey": "<account-key>",             // required when `Mode` is `SharedKey`
        "Timeout": "<timeout>"                     // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `TableServiceAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddTableServiceAvailability("<name>", options =>
{
    options.KeyedService = "<service-key>";           // optional, used when multiple clients are registered
    options.ConnectionString = "<connection-string>"; // required when `Mode` is `ConnectionString`
    options.Mode = "<client-creation-mode>";          // required to specify the client creation mode
    options.ServiceUri = "<service-uri>",             // required when `Mode` is `DefaultAzureCredentials`, `SharedKey` or `AzureSasCredential`
    options.AccountName = "<account-name>",           // required when `Mode` is `SharedKey`
    options.AccountKey = "<account-key>",             // required when `Mode` is `SharedKey`
    options.Timeout = "<timeout>"                     // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddTableServiceAvailability("<name>", options => ..., "azure");
```


## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
