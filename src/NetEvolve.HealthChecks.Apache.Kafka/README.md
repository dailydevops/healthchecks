# NetEvolve.HealthChecks.Apache.Kafka

[![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Apache.Kafka?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Kafka/)
[![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Apache.Kafka?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Kafka/)

This package provides a health check for Apache Kafka, based on the [Confluent.Kafka](https://www.nuget.org/packages/Confluent.Kafka/) package.
The main purpose is to check that the Kafka cluster is reachable and that the client can connect to it.

:bulb: This package is available for .NET 6.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Apache.Kafka
```

## Health Check - Apache Kafka Liveness
The health check is a liveness check. It will check that the Kafka cluster is reachable and that the client can connect to it.
If the cluster needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the cluster is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, yo need to import the namespace `NetEvolve.HealthChecks.Apache.Kafka` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Apache.Kafka;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `kafka` and `message-queue` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Kafka` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddKafka("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Kafka": {
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
The second approach is to use the builder based approach. This approach is recommended if you have only one Kafka instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddKafka("<name>", options =>
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
    builder.AddKafka("<name>", options => ..., "kafka");
```