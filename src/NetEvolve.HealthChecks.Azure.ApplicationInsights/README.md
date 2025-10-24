# NetEvolve.HealthChecks.Azure.ApplicationInsights

[![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.ApplicationInsights?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ApplicationInsights/)
[![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.ApplicationInsights?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ApplicationInsights/)

This package provides health checks for Azure Application Insights, allowing you to monitor the availability and connectivity of your telemetry tracking.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later
- Active Azure subscription
- Azure Application Insights resource created
- Instrumentation key or connection string
- Network connectivity to Azure Application Insights endpoints

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