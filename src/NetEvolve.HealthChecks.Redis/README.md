# NetEvolve.HealthChecks.Redis

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Redis?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redis/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Redis?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redis/)

This package provides a health check for Redis, based on the [StackExchange.Redis](https://www.nuget.org/packages/StackExchange.Redis/) package.
The main purpose is to check that the Redis is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Redis
```

## Health Check - Redis Liveness
The health check is a liveness check. It will check that the Redis is reachable and that the client can connect to it.
If the cluster needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the cluster is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Redis` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Redis;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `redis` and `cache` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Redis` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddRedis("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Redis": {
      "<name>": {
        "ConnectionString": "<connection string>", // required
        "Mode": "<producer handle mode>", // optional, Default ServiceProvider
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one Redis instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddRedis("<name>", options =>
{
    options.ConnectionString = "<connection string>"; // required
    options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
    ... // other configuration
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddRedis("<name>", options => ..., "redis");
```

## Troubleshooting

### Connection Timeouts

**Symptom**: Health check returns `Degraded` status with timeout errors.

**Possible Causes**:
- Network latency to cache server exceeds configured timeout
- Cache server is slow to respond under load
- Firewall or network configuration blocking connectivity
- DNS resolution delays

**Solutions**:
- Increase the `Timeout` value in health check configuration
- Check network connectivity using `ping` or `telnet`
- Verify cache server performance and resource availability
- Review firewall rules and security group settings
- Check DNS resolution for cache server hostname

### Authentication Failures

**Symptom**: Health check returns `Unhealthy` with authentication errors.

**Possible Causes**:
- Incorrect password in connection string
- Authentication not enabled on server but password provided
- Password authentication disabled on server
- Connection string format issues

**Solutions**:
- Verify password in connection string if authentication is enabled
- Remove password from connection string if authentication is disabled
- Check server authentication configuration
- Validate connection string format according to provider documentation
- Test connection using cache client tools (e.g., `redis-cli`)

### Cluster Configuration Issues

**Symptom**: Health check fails with cluster connectivity or configuration errors.

**Possible Causes**:
- Incorrect cluster endpoints
- Cluster mode vs standalone mode mismatch
- SSL/TLS configuration issues
- Cluster undergoing reconfiguration

**Solutions**:
- Verify cluster endpoint addresses and ports
- Ensure client configuration matches server mode (cluster/standalone)
- Configure SSL/TLS settings correctly if enabled
- Check cluster health and node status
- Review cluster configuration for recent changes

### Connection Pool Exhausted

**Symptom**: Health check intermittently fails with "connection pool exhausted" or similar errors.

**Possible Causes**:
- Application not properly disposing cache connections
- Connection pool size too small for application load
- Connection leaks in application code
- Too many concurrent operations

**Solutions**:
- Review application code for proper connection disposal
- Increase connection pool size in configuration
- Monitor active connections using cache server tools
- Use connection multiplexing where supported
- Check for connection leaks in application code

### SSL/TLS Connection Failures

**Symptom**: Health check fails with SSL/TLS handshake or certificate errors.

**Possible Causes**:
- SSL/TLS not enabled on server
- Certificate validation failing
- Incompatible SSL/TLS protocols
- Self-signed certificates not trusted

**Solutions**:
- Verify SSL/TLS is enabled on cache server
- Configure certificate validation settings appropriately
- Install required CA certificates on client
- Check SSL/TLS protocol versions compatibility
- Add `ssl=true` to connection string if required

### Configuration Not Found

**Symptom**: `InvalidOperationException` during startup with "Configuration for health check '<name>' not found" message.

**Possible Causes**:
- Configuration section missing from `appsettings.json`
- Mismatch between health check name and configuration section name
- Typos in configuration keys
- Wrong configuration file loaded

**Solutions**:
- Ensure configuration section exists in `appsettings.json`
- Verify the name used in `AddRedis("<name>")` matches the configuration section name
- Check for typos in configuration keys (case-sensitive)
- Verify correct `appsettings.json` file is being loaded for the environment

