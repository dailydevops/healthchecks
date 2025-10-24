# NetEvolve.HealthChecks.AWS.SQS

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.AWS.SQS?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SQS/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS.SQS?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SQS/)

This package provides a health check for AWS Simple Queue Service (SQS), based on the [AWS SDK for .NET](https://www.nuget.org/packages/AWSSDK.SQS/) package.
The main purpose is to check that the SQS service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later
- Active AWS account
- Amazon SQS queue created
- IAM credentials with `sqs:GetQueueAttributes` permission
- AWS SDK for .NET configured (via environment variables, AWS Profile, or instance role)

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.AWS.SQS
```

## Health Check - AWS SQS Liveness
The health check is a liveness check. It will check that the SQS service is reachable and that the client can connect to it.
If the service needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.AWS.SQS` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.AWS.SQS;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `aws`, `sqs` and `messaging` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AWSSQS` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSSQS("<name>");
```

The configuration looks like this:
```json
{
    ..., // other configuration
    "HealthChecks": {
        "AWSSQS": {
            "<name>": {
                "Region": "<aws-region>", // optional, uses default AWS region if not specified
                "AccessKey": "<access-key>", // optional, uses default AWS credentials if not specified
                "SecretKey": "<secret-key>", // optional, uses default AWS credentials if not specified
                "QueueName": "<queue-name>", // required
                "Timeout": "<timeout>" // optional, default is 100 milliseconds
            }
        }
    }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one SQS topic to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSSQS("<name>", options =>
{
        options.QueueName = "<queue-name>"; // required
        options.Region = "<aws-region>"; // optional, uses default AWS region if not specified
        options.AccessKey = "<access-key>"; // optional, uses default AWS credentials if not specified
        options.SecretKey = "<secret-key>"; // optional, uses default AWS credentials if not specified
        options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
        ... // other configuration
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAWSSQS("<name>", options => ..., "sqs");
```