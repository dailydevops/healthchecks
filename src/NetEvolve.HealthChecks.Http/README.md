# NetEvolve.HealthChecks.Http

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Http?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Http/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Http?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Http/)

This package contains a health check for HTTP endpoints, based on the `HttpClient`.
The health check verifies that HTTP endpoints respond with expected status codes within a configured timeout.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.

```powershell
dotnet add package NetEvolve.HealthChecks.Http
```

## Health Check - HTTP Endpoint Liveness
The health check is a liveness check that verifies an HTTP endpoint is reachable and responds with expected status codes.
If the endpoint takes longer than the configured timeout to respond, the health check will return `Degraded`.
If the endpoint is not reachable or returns an unexpected status code, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Http` and add the health check to the service collection.

```csharp
using NetEvolve.HealthChecks.Http;
```

You can use two different approaches to add the health check. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `http` and `endpoint` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Http` to your `appsettings.json` file.

```csharp
var builder = services.AddHealthChecks();

builder.AddHttp("<name>");
```

The configuration looks like this:

```json
{
    ..., // other configuration
    "HealthChecks": {
        "Http": {
            "<name>": {
                "Uri": "<endpoint-uri>", // required
                "HttpMethod": "<http-method>", // optional, default is "GET"
                "ExpectedHttpStatusCodes": [200, 201], // optional, default is [200]
                "Headers": { // optional, default is empty
                    "Authorization": "Bearer <token>",
                    "User-Agent": "HealthCheck/1.0"
                },
                "Timeout": "<timeout-in-ms>", // optional, default is 5000 milliseconds
                "Content": "<request-body>", // optional, default is null
                "ContentType": "<content-type>", // optional, default is "application/json"
                "AllowAutoRedirect": true // optional, default is true
            }
        }
    }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one HTTP endpoint to check or dynamic programmatic values.

```csharp
var builder = services.AddHealthChecks();

builder.AddHttp("<name>", options =>
{
    options.Uri = "<endpoint-uri>"; // required
    options.HttpMethod = "<http-method>"; // optional, default is "GET"
    options.ExpectedHttpStatusCodes = [200, 201]; // optional, default is [200]
    options.Headers["Authorization"] = "Bearer <token>"; // optional
    options.Timeout = 3000; // optional, default is 5000 milliseconds
    options.Content = "<request-body>"; // optional, default is null
    options.ContentType = "application/json"; // optional, default is "application/json"
    options.AllowAutoRedirect = true; // optional, default is true
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddHttp("<name>", options => ..., "http", "api");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
