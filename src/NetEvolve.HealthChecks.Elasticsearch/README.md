# NetEvolve.HealthChecks.Elasticsearch

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Elasticsearch?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Elasticsearch/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Elasticsearch?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Elasticsearch/)

This package provides a health check for Elasticsearch services, based on the [Elastic.Clients.Elasticsearch](https://www.nuget.org/packages/Elastic.Clients.Elasticsearch/) package. The main purpose is to check if the server is available and the service is online.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Elasticsearch
```

## Health Check - Elasticsearch Liveness
The health check is a liveness check. It checks if the Elasticsearch Server is available and if the service is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Elasticsearch;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `elasticsearch` and `searchengine` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple Elasticsearch instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddElasticsearch("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Elasticsearch": {
      "<name>": {
        "Mode": "<client_creation_mode>", // Optional, defaults to 'ElasticsearchClientCreationMode.ServiceProvider'
        "KeyedService": "<key>", // Optional, used when Mode set to 'ElasticsearchClientCreationMode.ServiceProvider'
        "Timeout": "<timeout>" // Optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one server instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddElasticsearch("<name>", options =>
{
    options.Mode = <client_creation_mode>; // Optional, defaults to 'ElasticsearchClientCreationMode.ServiceProvider'
    options.KeyedService = "<key>"; // Optional, used when Mode set to 'ElasticsearchClientCreationMode.ServiceProvider'
    options.Username = "<username>"; // Used when Mode set to 'ElasticsearchClientCreationMode.UsernameAndPassword' and required when Password is set
    options.Password = "<password>"; // Used when Mode set to 'ElasticsearchClientCreationMode.UsernameAndPassword' and required when Username is set
    options.Timeout = <timeout>; // Optional, defaults to 100 milliseconds
    
    foreach (var connectionString in connectionStrings) {
        options.ConnectionStrings.Add(connectionString); // Required when Mode set to 'ElasticsearchClientCreationMode.UsernameAndPassword'
    }

    // Optional, defaults to NetEvolve.HealthChecks.Elasticsearch.DefaultCommandAsync
    options.CommandAsync = async (client, cancellationToken) =>
    {
        // Your custom server pinging logic here.
        // Should return true if the command result is valid, false otherwise.
    };
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddElasticsearch("<name>", options => ..., "Elasticsearch", "elastic");
```

## Troubleshooting

### Connection Timeouts

**Symptom**: Health check returns `Degraded` status with "The operation has timed out" or similar message.

**Possible Causes**:
- Network latency between application and database server exceeds configured timeout
- Database server is slow to respond under load
- Firewall or network configuration blocking connectivity
- DNS resolution delays

**Solutions**:
- Increase the `Timeout` value in health check configuration
- Check network connectivity using `ping` or `telnet`
- Verify database server performance and resource availability
- Review firewall rules and security group settings
- Check DNS resolution for database server hostname

### Authentication Failures

**Symptom**: Health check returns `Unhealthy` with authentication or login failed errors.

**Possible Causes**:
- Incorrect credentials in connection string
- User account locked or disabled
- Insufficient permissions for health check operations
- Connection string format issues

**Solutions**:
- Verify username and password in connection string
- Check if user account is active and not locked
- Ensure user has `CONNECT` permission to the database
- Validate connection string format according to provider documentation
- Test credentials using database management tools

### Connection Pool Exhausted

**Symptom**: Health check intermittently fails with timeout or "connection pool exhausted" errors.

**Possible Causes**:
- Application not properly disposing database connections
- Connection pool size too small for application load
- Long-running queries blocking connections
- Connection leaks in application code

**Solutions**:
- Review application code for proper connection disposal
- Increase `Max Pool Size` in connection string
- Implement connection timeout in queries
- Monitor active connections using database tools
- Use `using` statements or `IDisposable` pattern properly

### SSL/TLS Certificate Issues

**Symptom**: Health check fails with SSL/TLS or certificate validation errors.

**Possible Causes**:
- Self-signed certificates not trusted
- Certificate expired or not yet valid
- Hostname mismatch in certificate
- TLS version incompatibility

**Solutions**:
- Add `TrustServerCertificate=True` to connection string (development only)
- Install proper CA certificates on the application server
- Verify certificate validity dates and hostname
- Update connection string to use correct hostname from certificate
- Ensure TLS 1.2 or higher is supported by both client and server

### Database Unavailable

**Symptom**: Health check returns `Unhealthy` with "database not found" or "cannot open database" errors.

**Possible Causes**:
- Database does not exist or is offline
- Incorrect database name in connection string
- Database is in recovery or maintenance mode
- Insufficient permissions to access database

**Solutions**:
- Verify database exists and is online
- Check database name spelling in connection string
- Wait for database recovery or maintenance to complete
- Ensure user has permission to access the specific database
- Check database server logs for errors

### Configuration Not Found

**Symptom**: `InvalidOperationException` during startup with "Configuration for health check '<name>' not found" message.

**Possible Causes**:
- Configuration section missing from `appsettings.json`
- Mismatch between health check name and configuration section name
- Typos in configuration keys
- Wrong configuration file loaded

**Solutions**:
- Ensure configuration section exists in `appsettings.json`
- Verify the name used in `Add<Database>("<name>")` matches the configuration section name
- Check for typos in configuration keys (case-sensitive)
- Verify correct `appsettings.json` file is being loaded for the environment

