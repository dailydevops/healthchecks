# NetEvolve.HealthChecks.Azure.Search

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Search?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Search/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Search?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Search/)

This package provides a health check for Azure AI Search (formerly Azure Cognitive Search), based on the [Azure.Search.Documents](https://www.nuget.org/packages/Azure.Search.Documents/) package. The main purpose is to check that the Azure Search service and a specific search index is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Search
```

## Health Check - Azure Search Index Availability
The health check is a liveness check. It will check that the Azure AI Search service and the specified search index is reachable and that the client can connect to it. If the service or the index needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service or the index is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Search` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Search;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `search`, `azure` and `cognitive` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:SearchAvailable` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureSearch("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "SearchAvailable": {
      "<name>": {
        "ServiceUri": "https://your-search-service.search.windows.net", // required
        "IndexName": "<index-name>", // required
        "Mode": "DefaultAzureCredentials", // optional, default is ServiceProvider
        "ApiKey": "<api-key>", // optional, required when Mode is AzureKeyCredential
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `SearchAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureSearch("<name>", options =>
{
    options.ServiceUri = new Uri("https://your-search-service.search.windows.net");
    options.IndexName = "<index-name>";
    options.Mode = ClientCreationMode.DefaultAzureCredentials;
    ...
    options.Timeout = "<timeout>";
});
```

## Configuration

### Client Creation Modes

The package supports three different modes to create the `SearchClient`:

- **ServiceProvider** (default): Retrieves the client from the service provider. This requires registering `SearchClient` in the dependency injection container.
- **DefaultAzureCredentials**: Creates a client using Azure Default Credentials. This is recommended for production environments and supports managed identities.
- **AzureKeyCredential**: Creates a client using an API key. This is useful for development and testing scenarios.

### Mode: ServiceProvider

When using the `ServiceProvider` mode, you need to register the `SearchClient` in the service collection:

```csharp
// Register SearchClient
services.AddSingleton(sp => 
    new SearchClient(
        new Uri("https://your-search-service.search.windows.net"),
        "your-index-name",
        new DefaultAzureCredential()));

// Add health check
services
    .AddHealthChecks()
    .AddAzureSearch(
        "SearchIndex",
        options =>
        {
            options.Mode = ClientCreationMode.ServiceProvider;
        });
```

You can also use keyed services for multiple search clients:

```csharp
// Register keyed SearchClient
services.AddKeyedSingleton("MySearchClient", (sp, key) => 
    new SearchClient(
        new Uri("https://your-search-service.search.windows.net"),
        "your-index-name",
        new DefaultAzureCredential()));

// Add health check with keyed service
services
    .AddHealthChecks()
    .AddAzureSearch(
        "SearchIndex",
        options =>
        {
            options.Mode = ClientCreationMode.ServiceProvider;
            options.KeyedService = "MySearchClient";
        });
```

### Mode: DefaultAzureCredentials

When using the `DefaultAzureCredentials` mode, the health check will create a `SearchClient` using the `DefaultAzureCredential`:

```csharp
services
    .AddHealthChecks()
    .AddAzureSearch(
        "SearchIndex",
        options =>
        {
            options.ServiceUri = new Uri("https://your-search-service.search.windows.net");
            options.IndexName = "your-index-name";
            options.Mode = ClientCreationMode.DefaultAzureCredentials;
        });
```

### Mode: AzureKeyCredential

When using the `AzureKeyCredential` mode, the health check will create a `SearchClient` using the provided API key:

```csharp
services
    .AddHealthChecks()
    .AddAzureSearch(
        "SearchIndex",
        options =>
        {
            options.ServiceUri = new Uri("https://your-search-service.search.windows.net");
            options.IndexName = "your-index-name";
            options.ApiKey = "your-api-key";
            options.Mode = ClientCreationMode.AzureKeyCredential;
        });
```

### Advanced Configuration

You can configure the `SearchClientOptions` by providing a configuration action:

```csharp
services
    .AddHealthChecks()
    .AddAzureSearch(
        "SearchIndex",
        options =>
        {
            options.ServiceUri = new Uri("https://your-search-service.search.windows.net");
            options.IndexName = "your-index-name";
            options.Mode = ClientCreationMode.DefaultAzureCredentials;
            options.ConfigureClientOptions = clientOptions =>
            {
                clientOptions.Retry.MaxRetries = 3;
                clientOptions.Retry.Delay = TimeSpan.FromSeconds(1);
            };
        });
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAzureSearch("<name>", options => ..., "azure");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
