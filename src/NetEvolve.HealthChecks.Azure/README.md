# NetEvolve.HealthChecks.Azure

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure/)

This bundle package provides health checks for various Azure services. For specific information about the health checks, please refer to the documentation of the individual packages.

:bulb: This package is available for .NET 8.0 and later.

## Supported Azure Services

- [Azure Blob Storage](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/)
- [Azure Service Bus](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ServiceBus/)
- [Azure Storage Queues](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/)
- [Azure Table Storage](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/)

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure
```

## Related Packages

### Individual Azure Services
This bundle package includes the following Azure service health checks:
- <a>`NetEvolve.HealthChecks.Azure.Blobs`</a> - Health checks for Azure Blob Storage
- <a>`NetEvolve.HealthChecks.Azure.Queues`</a> - Health checks for Azure Storage Queues
- <a>`NetEvolve.HealthChecks.Azure.Tables`</a> - Health checks for Azure Table Storage
- <a>`NetEvolve.HealthChecks.Azure.ServiceBus`</a> - Health checks for Azure Service Bus
- <a>`NetEvolve.HealthChecks.Azure.ApplicationInsights`</a> - Health checks for Azure Application Insights

You can install individual packages if you only need specific Azure service health checks.

### See Also
- <a>`NetEvolve.HealthChecks.Abstractions`</a> - Base abstractions for creating custom health checks