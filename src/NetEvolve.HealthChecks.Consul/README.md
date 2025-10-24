# NetEvolve.HealthChecks.Consul

![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Consul?logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Consul?logo=nuget)

This package provides health checks for HashiCorp Consul, based on the `Consul` NuGet package.

## Installation

```bash
dotnet add package NetEvolve.HealthChecks.Consul
```

## Usage

### Basic Setup

Register the Consul health check in your application:

```csharp
services
    .AddHealthChecks()
    .AddConsul("Consul");
```

### Configuration

The health check can be configured using the `ConsulOptions` class:

```csharp
services
    .AddHealthChecks()
    .AddConsul("Consul", options =>
    {
        options.Timeout = 1000; // Timeout in milliseconds
    });
```

#### Configuration via appsettings.json

```json
{
  "HealthChecks": {
    "Consul": {
      "MyConsulCheck": {
        "Timeout": 1000
      }
    }
  }
}
```

### Keyed Services

The health check supports keyed services for scenarios where multiple Consul clients are registered:

```csharp
services
    .AddKeyedSingleton<IConsulClient>("consul-primary", ...)
    .AddHealthChecks()
    .AddConsul("Consul", options =>
    {
        options.KeyedService = "consul-primary";
    });
```

## Health Check Behavior

The health check verifies:
- Connectivity to the Consul server
- Ability to query the Consul leader status

The check returns:
- `Healthy` if the Consul server responds successfully within the timeout period
- `Degraded` if the operation times out
- `Unhealthy` if the server is unreachable or returns an invalid response

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

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

