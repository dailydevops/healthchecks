# NetEvolve.HealthChecks.Azure.Cosmos

[![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Cosmos?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Cosmos/)
[![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Cosmos?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Cosmos/)

This package provides health checks for Azure Cosmos DB, based on the [Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Microsoft.Azure.Cosmos/) package. The main purpose is to check that Azure Cosmos DB services, databases, and containers are reachable and that the client can connect to them.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Cosmos
```

## Health Checks

### Azure Cosmos Client Availability
This health check verifies that the Azure Cosmos DB service is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Azure Cosmos Database Availability
This health check verifies that a specific Azure Cosmos DB database is reachable and that the client can connect to it. If the database needs longer than the configured timeout to respond, the health check will return `Degraded`. If the database is not reachable, the health check will return `Unhealthy`.

### Azure Cosmos Container Availability
This health check verifies that a specific Azure Cosmos DB container is reachable and that the client can connect to it. If the container needs longer than the configured timeout to respond, the health check will return `Degraded`. If the container is not reachable, the health check will return `Unhealthy`.

## Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Cosmos` and add the health check(s) to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Cosmos;
```
You can use two different approaches. In both approaches, you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `cosmos`, and `cosmosdb` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the appropriate configuration section to your `appsettings.json` file.

#### For Cosmos Client health check:
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosClientAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureCosmos": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required for ConnectionString mode
        "EndpointUri": "<endpoint-uri>", // required for DefaultAzureCredentials mode
        "PrimaryKey": "<primary-key>", // required when not using DefaultAzureCredentials
        "Mode": "ConnectionString", // ConnectionString, DefaultAzureCredentials, or ServiceProvider
        "Timeout": 100 // optional, default is 100 milliseconds
      }
    }
  }
}
```

#### For Cosmos Database health check:
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosDatabaseAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureCosmosDatabase": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required for ConnectionString mode
        "EndpointUri": "<endpoint-uri>", // required for DefaultAzureCredentials mode
        "PrimaryKey": "<primary-key>", // required when not using DefaultAzureCredentials
        "DatabaseId": "<database-id>", // required
        "Mode": "ConnectionString", // ConnectionString, DefaultAzureCredentials, or ServiceProvider
        "Timeout": 100 // optional, default is 100 milliseconds
      }
    }
  }
}
```

#### For Cosmos Container health check:
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosContainerAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureCosmosContainer": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required for ConnectionString mode
        "EndpointUri": "<endpoint-uri>", // required for DefaultAzureCredentials mode
        "PrimaryKey": "<primary-key>", // required when not using DefaultAzureCredentials
        "DatabaseId": "<database-id>", // required
        "ContainerId": "<container-id>", // required
        "Mode": "ConnectionString", // ConnectionString, DefaultAzureCredentials, or ServiceProvider
        "Timeout": 100 // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second approach is to use the options based approach. You create the configuration programmatically:

#### For Cosmos Client health check:
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosClientAvailability("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    // OR
    options.EndpointUri = new Uri("<endpoint-uri>");
    options.PrimaryKey = "<primary-key>";
    // OR
    options.Mode = CosmosClientCreationMode.ServiceProvider; // Use pre-registered CosmosClient
    
    options.Timeout = 100; // milliseconds
});
```

#### For Cosmos Database health check:
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosDatabaseAvailability("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    // OR
    options.EndpointUri = new Uri("<endpoint-uri>");
    options.PrimaryKey = "<primary-key>";
    // OR
    options.Mode = CosmosClientCreationMode.ServiceProvider; // Use pre-registered CosmosClient
    
    options.DatabaseId = "<database-id>";
    options.Timeout = 100; // milliseconds
});
```

#### For Cosmos Container health check:
```csharp
var builder = services.AddHealthChecks();

builder.AddCosmosContainerAvailability("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    // OR
    options.EndpointUri = new Uri("<endpoint-uri>");
    options.PrimaryKey = "<primary-key>";
    // OR
    options.Mode = CosmosClientCreationMode.ServiceProvider; // Use pre-registered CosmosClient
    
    options.DatabaseId = "<database-id>";
    options.ContainerId = "<container-id>";
    options.Timeout = 100; // milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering:

```csharp
var builder = services.AddHealthChecks();
builder.AddCosmosClientAvailability("<name>", options => { ... }, "nosql", "database");
```