# NetEvolve.HealthChecks.AWS.DynamoDB

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.AWS.DynamoDB?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.DynamoDB/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS.DynamoDB?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.DynamoDB/)

This package provides a health check for AWS DynamoDB, based on the [AWS SDK for .NET](https://www.nuget.org/packages/AWSSDK.DynamoDBv2/) package.
The main purpose is to check that the DynamoDB service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later
- Active AWS account
- Amazon DynamoDB table created and accessible
- IAM credentials with `dynamodb:DescribeTable` permission
- AWS SDK for .NET configured (via environment variables, AWS Profile, or instance role)

## Installation
To use this package, you need to install the `NetEvolve.HealthChecks.AWS.DynamoDB` NuGet package. You can do this by running the following command:

```bash
dotnet add package NetEvolve.HealthChecks.AWS.DynamoDB
```

## Health Check - AWS DynamoDB Liveness
The health check verifies that a specified DynamoDB table exists and is accessible by performing a DescribeTable operation.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.AWS.DynamoDB` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.AWS.DynamoDB;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `aws`, `dynamodb` and `database` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AWSDynamoDB` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSDynamoDB("<name>");
```

The configuration looks like this:
```json
{
    ..., // other configuration
    "HealthChecks": {
        "AWSDynamoDB": {
            "<name>": {
                "Region": "<aws-region>", // optional, uses default AWS region if not specified
                "AccessKey": "<access-key>", // optional, uses default AWS credentials if not specified
                "SecretKey": "<secret-key>", // optional, uses default AWS credentials if not specified
                "TableName": "<table-name>", // required
                "Timeout": "<timeout>" // optional, default is 100 milliseconds
            }
        }
    }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one DynamoDB table to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddAWSDynamoDB("<name>", options =>
{
        options.TableName = "<table-name>"; // required
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

builder.AddAWSDynamoDB("<name>", options => ..., "dynamodb");
```
