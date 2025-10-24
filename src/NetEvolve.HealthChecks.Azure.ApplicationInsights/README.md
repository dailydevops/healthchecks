# NetEvolve.HealthChecks.Azure.ApplicationInsights

[![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.ApplicationInsights?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ApplicationInsights/)
[![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.ApplicationInsights?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ApplicationInsights/)

This package provides health checks for Azure Application Insights, allowing you to monitor the availability and connectivity of your telemetry tracking.

:bulb: This package is available for .NET 6.0 and later.

## Installation

To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.

```powershell
dotnet add package NetEvolve.HealthChecks.Azure.ApplicationInsights
```

## Health Check - Azure Application Insights Availability

The health check checks the availability of Azure Application Insights by attempting to track a test telemetry event.

### Usage

After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.ApplicationInsights` and add the health check to the service collection.

```csharp
using NetEvolve.HealthChecks.Azure.ApplicationInsights;
```

Depending on the configuration approach, you can use two different approaches.

### Parameters

- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health checks. The tags `azure`, `applicationinsights`, and `telemetry` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based

The first approach is to use the configuration-based method. Therefore, you have to add the configuration section `HealthChecks:ApplicationInsightsAvailability` to your `appsettings.json` file.

```csharp
var builder = services.AddHealthChecks();

builder.AddApplicationInsightsAvailability("<name>");
```

The configuration looks like this:

```json
{
  ..., // other configuration
  "HealthChecks": {
    "ApplicationInsightsAvailability": {
      "<name>": {
        "ConnectionString": "<connection-string>", // required
        "Mode": "<mode>", // required, to specify the client creation mode, either `ConnectionString`, `InstrumentationKey`, or `ServiceProvider`
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based

The second approach is to use the options-based method. Therefore, you have to create an instance of `ApplicationInsightsAvailabilityOptions` and provide the configuration.

```csharp
var builder = services.AddHealthChecks();

builder.AddApplicationInsightsAvailability("<name>", options =>
{
    options.ConnectionString = "<connection-string>";
    options.Mode = ApplicationInsightsClientCreationMode.ConnectionString;
    ...
    options.Timeout = "<timeout>";
});
```

### Client Creation Modes

The health check supports different modes for creating the Application Insights TelemetryClient:

#### ConnectionString

Use this mode when you have an Application Insights connection string.

```csharp
builder.AddApplicationInsightsAvailability("<name>", options =>
{
    options.Mode = ApplicationInsightsClientCreationMode.ConnectionString;
    options.ConnectionString = "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/";
});
```

**Configuration:**

```json
{
  "HealthChecks": {
    "ApplicationInsightsAvailability": {
      "<name>": {
        "Mode": "ConnectionString",
        "ConnectionString": "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/"
      }
    }
  }
}
```

#### InstrumentationKey

Use this mode when you have an Application Insights instrumentation key.

```csharp
builder.AddApplicationInsightsAvailability("<name>", options =>
{
    options.Mode = ApplicationInsightsClientCreationMode.InstrumentationKey;
    options.InstrumentationKey = "12345678-1234-1234-1234-123456789abc";
});
```

**Configuration:**

```json
{
  "HealthChecks": {
    "ApplicationInsightsAvailability": {
      "<name>": {
        "Mode": "InstrumentationKey",
        "InstrumentationKey": "12345678-1234-1234-1234-123456789abc"
      }
    }
  }
}
```

#### ServiceProvider

Use this mode when you have already registered a `TelemetryClient` in the service provider (e.g., using `AddApplicationInsightsTelemetry()`).

```csharp
// First register Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Then add the health check
builder.AddApplicationInsightsAvailability("<name>", options =>
{
    options.Mode = ApplicationInsightsClientCreationMode.ServiceProvider;
});
```

**Configuration:**

```json
{
  "HealthChecks": {
    "ApplicationInsightsAvailability": {
      "<name>": {
        "Mode": "ServiceProvider"
      }
    }
  }
}
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddApplicationInsightsAvailability("<name>", options => ..., "custom", "azure");
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

