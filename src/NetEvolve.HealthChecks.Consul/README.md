# NetEvolve.HealthChecks.Consul

![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Consul?logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Consul?logo=nuget)

This package provides health checks for HashiCorp Consul, based on the `Consul` NuGet package.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later
- HashiCorp Consul 1.10 or later (recommended: 1.16+)
- Network connectivity to Consul agent
- Valid ACL token if ACL is enabled

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
