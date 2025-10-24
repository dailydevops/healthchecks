# NetEvolve.HealthChecks.Azure.Blobs

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Blobs?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Blobs?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/)

This package provides a health check for Azure Blobs, based on the [Azure.Storage.Blobs](https://www.nuget.org/packages/Azure.Storage.Blobs/) package. The main purpose is to check that the Azure Blob Service and the Storage Container is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.Blobs
```

## Health Check - Azure Blob Container Availability
The health check is a liveness check. It will check that the Azure Blob Service and the Storage Container is reachable and that the client can connect to it. If the service or the container needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service or the container is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.Blobs` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.Blobs;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `storage` and `blob` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureBlobContainer` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureBlobContainer("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureBlobContainer": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required
        "ContainerName": "<container-name>", // required
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `AzureBlobContainerOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureBlobContainer("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.ContainerName = "<container-name>";
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAzureBlobContainer("<name>", options => ..., "azure");
```

## Troubleshooting

### Connection Timeouts

**Symptom**: Health check returns `Degraded` status with timeout errors.

**Possible Causes**:
- Network latency to Azure service endpoint exceeds configured timeout
- Azure service experiencing high latency
- Firewall or NSG blocking connectivity
- DNS resolution delays for private endpoints

**Solutions**:
- Increase the `Timeout` value in health check configuration
- Check network connectivity to Azure service endpoints
- Verify Azure service status in Azure Service Health
- Review NSG rules and Azure Firewall settings
- Check VNet peering and DNS configuration for private endpoints

### Azure Authentication Failures

**Symptom**: Health check fails with Azure authentication or authorization errors.

**Possible Causes**:
- DefaultAzureCredential not properly configured
- Managed Identity not enabled or assigned
- Connection string credentials invalid
- Insufficient RBAC permissions

**Solutions**:
- Enable Managed Identity on Azure resource (App Service, VM, Container Instance, etc.)
- Assign appropriate RBAC role to Managed Identity (e.g., `Storage Blob Data Contributor`)
- Verify connection string credentials are correct
- Check `Mode` setting (`ServiceProvider`, `DefaultAzureCredentials`, or `ConnectionString`)
- Review Azure Active Directory sign-in logs for authentication details

### Resource Not Found

**Symptom**: Health check fails with "resource not found" or 404 errors.

**Possible Causes**:
- Resource name incorrect or typo
- Resource in different subscription or resource group
- Resource deleted or not yet created
- Case sensitivity in resource names

**Solutions**:
- Verify exact resource names using Azure Portal or Azure CLI
- Check resource exists in expected subscription and resource group
- Validate resource naming conventions (some are case-sensitive)
- Ensure resource is fully provisioned before health check runs

### Network Connectivity Issues

**Symptom**: Health check fails with network timeout or connection refused errors.

**Possible Causes**:
- Private endpoint enabled but not configured properly
- NSG rules blocking traffic
- Service firewall rules excluding application IP
- VNet peering or routing issues

**Solutions**:
- Configure private endpoint access if resource uses private endpoints
- Review NSG rules and allow required ports/protocols
- Add application IP to resource firewall allow list
- Verify VNet integration and DNS resolution for private endpoints
- Check Azure service health status for regional issues

### RBAC Permission Denied

**Symptom**: Health check returns `Unhealthy` with "Forbidden" or authorization errors.

**Possible Causes**:
- Managed Identity missing required RBAC role
- Role assignment not yet propagated
- Insufficient permissions for health check operations
- Role assigned at wrong scope

**Solutions**:
- Assign required RBAC role to Managed Identity or Service Principal
- Wait a few minutes for role assignments to propagate
- Verify role has necessary permissions for the operation
- Check role is assigned at correct scope (resource, resource group, or subscription)
- Use Azure Activity Log to troubleshoot authorization failures

### Configuration Not Found

**Symptom**: `InvalidOperationException` during startup with "Configuration for health check '<name>' not found" message.

**Possible Causes**:
- Configuration section missing from `appsettings.json`
- Mismatch between health check name and configuration section name
- Typos in configuration keys
- Wrong configuration file loaded

**Solutions**:
- Ensure configuration section exists in `appsettings.json`
- Verify the name used in `AddAzure<Service>("<name>")` matches the configuration section name
- Check for typos in configuration keys (case-sensitive)
- Verify correct `appsettings.json` file is being loaded for the environment

