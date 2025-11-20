# NetEvolve.HealthChecks.Azure.Search

This package provides health checks for Azure AI Search (formerly Azure Cognitive Search).

## Installation

Install the package via NuGet:

```bash
dotnet add package NetEvolve.HealthChecks.Azure.Search
```

## Usage

### Search Service Health Check

Check the availability of the Azure Search service by verifying that it can list index names.

```csharp
services
    .AddHealthChecks()
    .AddSearchServiceAvailability(
        "SearchService",
        options =>
        {
            options.ServiceUri = new Uri("https://your-search-service.search.windows.net");
            options.ApiKey = "your-api-key";
            options.Mode = ClientCreationMode.AzureKeyCredential;
        });
```

### Search Index Health Check

Check the availability of a specific search index by verifying that it can retrieve the document count.

```csharp
services
    .AddHealthChecks()
    .AddSearchIndexAvailability(
        "SearchIndex",
        options =>
        {
            options.ServiceUri = new Uri("https://your-search-service.search.windows.net");
            options.ApiKey = "your-api-key";
            options.IndexName = "your-index-name";
            options.Mode = ClientCreationMode.AzureKeyCredential;
        });
```

## Configuration

### Client Creation Modes

- **ServiceProvider**: Retrieves the client from the service provider (requires registering `SearchIndexClient` or `SearchClient` in DI)
- **DefaultAzureCredentials**: Creates a client using Azure Default Credentials (recommended for production)
- **AzureKeyCredential**: Creates a client using an API key (useful for development and testing)

### Using Default Azure Credentials

```csharp
services
    .AddHealthChecks()
    .AddSearchServiceAvailability(
        "SearchService",
        options =>
        {
            options.ServiceUri = new Uri("https://your-search-service.search.windows.net");
            options.Mode = ClientCreationMode.DefaultAzureCredentials;
        });
```

### Using Service Provider

```csharp
services.AddSingleton<SearchIndexClient>(sp => 
    new SearchIndexClient(
        new Uri("https://your-search-service.search.windows.net"),
        new DefaultAzureCredential()));

services
    .AddHealthChecks()
    .AddSearchServiceAvailability(
        "SearchService",
        options =>
        {
            options.Mode = ClientCreationMode.ServiceProvider;
        });
```

### Configuration via appsettings.json

```json
{
  "SearchService": {
    "ServiceUri": "https://your-search-service.search.windows.net",
    "ApiKey": "your-api-key",
    "Mode": "AzureKeyCredential",
    "Timeout": 100
  },
  "SearchIndex": {
    "ServiceUri": "https://your-search-service.search.windows.net",
    "ApiKey": "your-api-key",
    "IndexName": "your-index-name",
    "Mode": "AzureKeyCredential",
    "Timeout": 100
  }
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.
