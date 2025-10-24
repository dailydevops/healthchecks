# NetEvolve.HealthChecks.Abstractions

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Abstractions?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Abstractions?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/)

This package provides a set of abstractions for creating health checks in .NET applications. It is based on the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks/) package and used for creating the NetEvolve.HealthChecks libraries.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Abstractions
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

