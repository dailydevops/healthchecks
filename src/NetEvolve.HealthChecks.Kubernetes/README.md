# NetEvolve.HealthChecks.Kubernetes

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Kubernetes?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Kubernetes/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Kubernetes?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Kubernetes/)

This package provides health checks for Kubernetes, based on the `KubernetesClient` NuGet package.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Kubernetes
```

## Health Check - Kubernetes Liveness
The health check is a liveness check. It verifies connectivity to the Kubernetes cluster and the ability to query the Kubernetes version.
If the operation needs longer than the configured timeout, the health check will return `Degraded`.
If the cluster is unreachable or returns an invalid response, the health check will return `Unhealthy`.

## Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Kubernetes;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `kubernetes` and `k8s` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple Kubernetes clusters to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddKubernetes("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Kubernetes": {
      "<name>": {
        "Timeout": 1000 // Optional, timeout in milliseconds, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one Kubernetes cluster to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddKubernetes("<name>", options =>
{
    options.Timeout = 1000; // Optional, timeout in milliseconds, default is 100 milliseconds
    options.KeyedService = "<key>"; // Optional, used when multiple Kubernetes clients are registered
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddKubernetes("<name>", options => ..., "kubernetes", "k8s");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
