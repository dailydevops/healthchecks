# NetEvolve.HealthChecks.AWS.EC2

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.AWS.EC2?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.EC2/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS.EC2?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.EC2/)

This package provides a health check for AWS Elastic Compute Cloud (EC2), based on the [AWS SDK for .NET](https://www.nuget.org/packages/AWSSDK.EC2/) package.
The main purpose is to check that the EC2 service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.AWS.EC2
```

## Health Check - AWS EC2 Liveness
The health check is a liveness check. It will check that the EC2 service is reachable and that the client can connect to it.
If the service needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.AWS.EC2;
```

Optionally, you can also add the namespace globally to your project by adding the following to your `GlobalUsings.cs` file.
```csharp
global using NetEvolve.HealthChecks.AWS.EC2;
```

### Parameters
- `name`: The name of the health check. The name is used to identify the health check in the UI. **Required**.
- `options`: The configuration options for the health check. **Optional**.
- `tags`: The tags for the health check. **Optional**.
  - `aws`: This tag is always assigned to the health check.
  - `ec2`: This tag is always assigned to the health check.
  - `compute`: This tag is always assigned to the health check.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AWSEC2` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSEC2("<name>");
```

The configuration looks like this:
```json
{
    ..., // other configuration
    "HealthChecks": {
        "AWSEC2": {
            "<name>": {
                "Region": "<aws-region>", // optional, uses default AWS region if not specified
                "AccessKey": "<access-key>", // optional, uses default AWS credentials if not specified
                "SecretKey": "<secret-key>", // optional, uses default AWS credentials if not specified
                "Timeout": "<timeout>" // optional, default is 100 milliseconds
            }
        }
    }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one EC2 service to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSEC2("<name>", options =>
{
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

builder.AddAWSEC2("<name>", options => ..., "ec2");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
