# NetEvolve.HealthChecks.Azure.Kusto

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Kusto?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Kusto/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Kusto?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Kusto/)

This package provides health checks for Azure Kusto (Data Explorer) clusters, allowing you to monitor cluster connectivity and database availability.

:bulb: This package is available for .NET 8.0 and later.

## Installation

To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.

```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Kusto
```

## Health Check - Azure Kusto

The health check is a liveness check that verifies connectivity to the Azure Kusto cluster. It can optionally check for the availability of a specific database. If the cluster is unreachable or the query takes longer than the configured timeout, the health check will return `Degraded`. If the cluster or database is not available, the health check will return `Unhealthy`.

### Usage

After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Kusto` and add the health check to the service collection.

```csharp
using NetEvolve.HealthChecks.Azure.Kusto;
```

Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters

- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `kusto`, `data` and `analytics` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based

The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureKusto` to your `appsettings.json` file.

```csharp
var builder = services.AddHealthChecks();

builder.AddKusto("<name>");
```

The configuration looks like this:

```json
{
  // ... other configuration
  "HealthChecks": {
    "AzureKusto": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required when using ConnectionString mode
        "ClusterUri": "<cluster-uri>", // required when using DefaultAzureCredentials mode
        "DatabaseName": "<database-name>", // optional, if specified, checks database availability
        "Mode": "ConnectionString", // optional, defaults to ConnectionString
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based

The second one is to use the options based approach. Therefore, you have to create an instance of `KustoOptions` and provide the configuration.

```csharp
var builder = services.AddHealthChecks();

builder.AddKusto("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.DatabaseName = "<database-name>"; // optional
    options.Timeout = 1000; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddKusto("<name>", options => ..., "kusto", "cluster");
```

## Configuration

### Supported Creation Modes

- **ServiceProvider**: Uses a pre-registered `ICslQueryProvider` from the service collection.
- **ConnectionString**: Creates a Kusto client using a connection string.
- **DefaultAzureCredentials**: Uses Azure Default Credentials for authentication with the cluster URI.

### Timeout Configuration

The timeout specifies how long to wait for the Kusto query to complete. The default timeout is 100 milliseconds. Values below -1 (infinite timeout) are invalid.

### Database Validation

If you specify a `DatabaseName` in the options, the health check will verify that the database exists on the cluster. If no database name is provided, the health check only verifies cluster connectivity.