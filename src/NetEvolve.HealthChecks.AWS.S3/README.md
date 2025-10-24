# NetEvolve.HealthChecks.AWS.S3

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.AWS.S3?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.S3/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS.S3?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.S3/)

This package provides a health check for AWS Simple Storage Service (S3), based on the [AWS SDK for .NET](https://www.nuget.org/packages/AWSSDK.S3/) package.
The main purpose is to check that the S3 service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later
- Active AWS account
- Amazon S3 bucket created and accessible
- IAM credentials with `s3:GetBucketLocation` permission
- AWS SDK for .NET configured (via environment variables, AWS Profile, or instance role)

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.AWS.S3
```

## Health Check - AWS S3 Liveness
The health check is a liveness check. It will check that the S3 service is reachable and that the client can connect to it.
If the service needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.AWS.S3` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.AWS.S3;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `aws`, `s3` and `storage` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AWSS3` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSS3("<name>");
```

The configuration looks like this:
```json
{
    ..., // other configuration
    "HealthChecks": {
        "AWSS3": {
            "<name>": {
                "Region": "<aws-region>", // optional, uses default AWS region if not specified
                "AccessKey": "<access-key>", // optional, uses default AWS credentials if not specified
                "SecretKey": "<secret-key>", // optional, uses default AWS credentials if not specified
                "BucketName": "<bucket-name>", // required
                "Timeout": "<timeout>" // optional, default is 100 milliseconds
            }
        }
    }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one S3 bucket to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSS3("<name>", options =>
{
        options.BucketName = "<bucket-name>"; // required
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

builder.AddAWSS3("<name>", options => ..., "s3");
```