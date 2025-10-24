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

## Troubleshooting

### Connection Timeouts

**Symptom**: Health check returns `Degraded` status with timeout errors.

**Possible Causes**:
- Network latency to service endpoint exceeds configured timeout
- Service is slow to respond under load
- Firewall or network configuration blocking connectivity
- DNS resolution delays

**Solutions**:
- Increase the `Timeout` value in health check configuration
- Check network connectivity to service endpoints
- Verify service performance and resource availability
- Review firewall rules and security group settings
- Check DNS resolution for service hostnames

### Service Unavailable

**Symptom**: Health check returns `Unhealthy` with connection refused or service unavailable errors.

**Possible Causes**:
- Service is not running or not started
- Incorrect service endpoint or port in configuration
- Network routing issues or DNS failure
- Service is running but not accepting connections

**Solutions**:
- Verify service is running and accessible
- Test connectivity using service-specific tools
- Validate endpoint configuration (hostname, port, protocol)
- Check service logs for startup errors
- Verify network routing and DNS resolution

### Authentication Failures

**Symptom**: Health check returns `Unhealthy` with authentication or authorization errors.

**Possible Causes**:
- Incorrect credentials in configuration
- Authentication method not supported
- Expired tokens or certificates
- Insufficient permissions for health check operations

**Solutions**:
- Verify authentication credentials in configuration
- Check supported authentication methods for the service
- Renew expired tokens or certificates
- Ensure service account has necessary permissions
- Review service logs for detailed authentication failures

### Configuration Not Found

**Symptom**: `InvalidOperationException` during startup with "Configuration for health check '<name>' not found" message.

**Possible Causes**:
- Configuration section missing from `appsettings.json`
- Mismatch between health check name and configuration section name
- Typos in configuration keys
- Wrong configuration file loaded

**Solutions**:
- Ensure configuration section exists in `appsettings.json`
- Verify the name used in health check registration matches the configuration section name
- Check for typos in configuration keys (case-sensitive)
- Verify correct `appsettings.json` file is being loaded for the environment

### SSL/TLS Connection Issues

**Symptom**: Health check fails with SSL/TLS or certificate validation errors.

**Possible Causes**:
- SSL/TLS not properly configured
- Certificate validation failing
- Self-signed certificates not trusted
- TLS version incompatibility

**Solutions**:
- Verify SSL/TLS configuration on both client and service
- Install required CA certificates on client
- Configure certificate validation settings appropriately
- Check TLS version compatibility between client and service
- Review service SSL/TLS requirements

