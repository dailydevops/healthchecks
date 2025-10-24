# NetEvolve.HealthChecks.Redpanda

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Redpanda?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redpanda/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Redpanda?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redpanda/)

This package provides a health check for Redpanda, based on the [Confluent.Kafka](https://www.nuget.org/packages/Confluent.Kafka/) package. This is a temporary measure; if a dedicated Redpanda client is provided in the future, we will use it immediately.
The main purpose is to check that the Redpanda cluster is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Redpanda
```

## Health Check - Redpanda Liveness
The health check is a liveness check. It will check that the Redpanda cluster is reachable and that the client can connect to it.
If the cluster needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the cluster is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Redpanda` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Redpanda;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `redpanda` and `messaging` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Redpanda` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddRedpanda("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Redpanda": {
      "<name>": {
        "Configuration": {
          "BootstrapServers": "<bootstrap-servers>", // required
          ..., // other configuration
        }",
        "Mode": "<producer handle mode>", // optional, Default ServiceProvider
        "Topic": "<topic>", // required
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one Redpanda instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddRedpanda("<name>", options =>
{
    options.Configuration.BootstrapServers = "<bootstrap-servers>"; // required
    options.Topic = "<topic>"; // required
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
    ... // other configuration
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddRedpanda("<name>", options => ..., "redpanda");
```

## Troubleshooting

### Connection Timeouts

**Symptom**: Health check returns `Degraded` status with timeout errors.

**Possible Causes**:
- Network latency to message broker exceeds configured timeout
- Broker is slow to respond under load
- Firewall or network configuration blocking connectivity
- DNS resolution delays

**Solutions**:
- Increase the `Timeout` value in health check configuration
- Check network connectivity to broker endpoints
- Verify broker performance and resource availability
- Review firewall rules and security group settings
- Check DNS resolution for broker hostnames

### Authentication Failures

**Symptom**: Health check returns `Unhealthy` with authentication or authorization errors.

**Possible Causes**:
- Incorrect credentials or tokens in configuration
- Authentication method not supported or misconfigured
- Expired certificates or keys
- Insufficient permissions for health check operations

**Solutions**:
- Verify authentication credentials in configuration
- Check supported authentication methods (SASL, SSL, etc.)
- Renew expired certificates or authentication tokens
- Ensure service account has necessary permissions (describe, list topics/queues)
- Review broker logs for detailed authentication failures

### Topic/Queue Does Not Exist

**Symptom**: Health check fails with "topic not found", "queue does not exist" or similar errors.

**Possible Causes**:
- Topic/queue name incorrect or typo
- Topic/queue not created yet
- Case sensitivity in naming
- Different namespace/cluster being accessed

**Solutions**:
- Verify topic/queue exists using broker management console
- Create topic/queue before starting application
- Check exact naming including case sensitivity
- Validate broker endpoint matches where topic/queue exists
- Use broker CLI tools to list available topics/queues

### Message Broker Unreachable

**Symptom**: Health check consistently returns `Unhealthy` with connection errors.

**Possible Causes**:
- Broker service not running
- Incorrect broker address or port
- Network or firewall issues
- Broker at capacity or rejecting connections

**Solutions**:
- Verify broker service is running and healthy
- Validate broker address and port in configuration
- Test network connectivity using `telnet` or `nc`
- Check broker logs for connection rejections
- Monitor broker resource usage (CPU, memory, connections)
- Verify broker connection limits not exceeded

### SSL/TLS Connection Failures

**Symptom**: Health check fails with SSL/TLS handshake or certificate errors.

**Possible Causes**:
- SSL/TLS not enabled on broker
- Certificate validation failing
- Incompatible SSL/TLS protocols
- Missing or incorrect CA certificates

**Solutions**:
- Verify SSL/TLS is enabled on broker
- Configure certificate validation settings appropriately
- Install required CA certificates on client
- Check SSL/TLS protocol versions match between client and broker
- Review broker SSL/TLS configuration

### Configuration Not Found

**Symptom**: `InvalidOperationException` during startup with "Configuration for health check '<name>' not found" message.

**Possible Causes**:
- Configuration section missing from `appsettings.json`
- Mismatch between health check name and configuration section name
- Typos in configuration keys
- Wrong configuration file loaded

**Solutions**:
- Ensure configuration section exists in `appsettings.json`
- Verify the name used in `Add<Broker>("<name>")` matches the configuration section name
- Check for typos in configuration keys (case-sensitive)
- Verify correct `appsettings.json` file is being loaded for the environment

