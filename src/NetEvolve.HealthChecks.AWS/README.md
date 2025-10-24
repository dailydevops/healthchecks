# NetEvolve.HealthChecks.AWS

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.AWS?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS/)

This bundle package provides health checks for various AWS services. For specific information about the health checks, please refer to the documentation of the individual packages.

:bulb: This package is available for .NET 8.0 and later.

## Supported AWS Services

- [AWS Elastic Compute Cloud (EC2)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.EC2/)
- [AWS Simple Notification Service (SNS)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SNS/)
- [AWS Simple Queue Service (SQS)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SQS/)
- [AWS Simple Storage Service (S3)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.S3/)

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.AWS
```

## Related Packages

### Individual AWS Services
This bundle package includes the following AWS service health checks:
- <a>`NetEvolve.HealthChecks.AWS.EC2`</a> - Health checks for AWS EC2
- <a>`NetEvolve.HealthChecks.AWS.S3`</a> - Health checks for AWS S3
- <a>`NetEvolve.HealthChecks.AWS.DynamoDB`</a> - Health checks for AWS DynamoDB
- <a>`NetEvolve.HealthChecks.AWS.SQS`</a> - Health checks for AWS SQS
- <a>`NetEvolve.HealthChecks.AWS.SNS`</a> - Health checks for AWS SNS

You can install individual packages if you only need specific AWS service health checks.

### See Also
- <a>`NetEvolve.HealthChecks.Abstractions`</a> - Base abstractions for creating custom health checks
