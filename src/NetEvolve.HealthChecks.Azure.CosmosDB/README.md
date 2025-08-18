# NetEvolve.HealthChecks.Azure.CosmosDB

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.CosmosDB?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.CosmosDB/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.CosmosDB?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.CosmosDB/)

This package provides a health check for Azure Cosmos DB, based on the [Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Microsoft.Azure.Cosmos/) package. The main purpose is to check that the Azure Cosmos DB service is available and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.CosmosDB
```

## Health Check - Azure Cosmos DB Availability
The health check is a liveness check. It will check that the Azure Cosmos DB service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.CosmosDB` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.CosmosDB;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `cosmosdb` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:CosmosDb` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosDb("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "CosmosDb": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required when using ConnectionString mode
        "ServiceEndpoint": "<service-endpoint>", // required when using other modes
        "AccountKey": "<account-key>", // required when using AccountKey mode
        "DatabaseName": "<database-name>", // optional, checks specific database
        "ContainerName": "<container-name>", // optional, checks specific container (requires DatabaseName)
        "Mode": "<mode>", // optional, defaults to ConnectionString
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `CosmosDbOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosDb("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.DatabaseName = "<database-name>"; // optional
    options.ContainerName = "<container-name>"; // optional
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosDb("<name>", options => ..., "cosmosdb");
```

### Client Creation Modes
The health check supports multiple authentication modes:

#### ConnectionString
Uses a Cosmos DB connection string for authentication.
```csharp
builder.AddCosmosDb("<name>", options =>
{
    options.Mode = CosmosDbClientCreationMode.ConnectionString;
    options.ConnectionString = "<connection-string>";
});
```

#### DefaultAzureCredentials
Uses Default Azure Credentials for authentication.
```csharp
builder.AddCosmosDb("<name>", options =>
{
    options.Mode = CosmosDbClientCreationMode.DefaultAzureCredentials;
    options.ServiceEndpoint = "<service-endpoint>";
});
```

#### AccountKey
Uses account key for authentication.
```csharp
builder.AddCosmosDb("<name>", options =>
{
    options.Mode = CosmosDbClientCreationMode.AccountKey;
    options.ServiceEndpoint = "<service-endpoint>";
    options.AccountKey = "<account-key>";
});
```

#### ServicePrincipal
Uses service principal for authentication (requires token credential to be registered).
```csharp
builder.AddCosmosDb("<name>", options =>
{
    options.Mode = CosmosDbClientCreationMode.ServicePrincipal;
    options.ServiceEndpoint = "<service-endpoint>";
});
```