# NetEvolve.HealthChecks.AWS.SQS

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.AWS.SQS?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SQS/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS.SQS?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SQS/)

This package provides a health check for AWS Simple Queue Service (SQS), based on the [AWS SDK for .NET](https://www.nuget.org/packages/AWSSDK.SQS/) package.
The main purpose is to check that the SQS service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

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

## Troubleshooting

### Connection Timeouts

**Symptom**: Health check returns `Degraded` status with timeout errors.

**Possible Causes**:
- Network latency to AWS service endpoint exceeds configured timeout
- AWS service experiencing high latency
- Firewall or security group blocking connectivity
- DNS resolution delays

**Solutions**:
- Increase the `Timeout` value in health check configuration
- Check network connectivity to AWS service endpoints
- Verify AWS service status in the AWS Health Dashboard
- Review security group and network ACL rules
- Check VPC routing and internet gateway configuration

### AWS Credentials Not Found

**Symptom**: Health check fails with "Unable to get IAM security credentials" or similar AWS authentication errors.

**Possible Causes**:
- AWS credentials not configured on the host
- IAM role not attached to EC2 instance or ECS task
- Credentials expired or invalid
- Insufficient IAM permissions

**Solutions**:
- Configure AWS credentials using AWS CLI: `aws configure`
- Verify IAM role is attached to EC2/ECS/Lambda resource
- Check credentials in `~/.aws/credentials` or environment variables (`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`)
- Validate IAM policy grants necessary permissions for the service
- Review CloudWatch logs for detailed error messages

### Region Configuration Issues

**Symptom**: Health check fails with "Region not found" or endpoint connection errors.

**Possible Causes**:
- AWS region not specified or incorrect
- Service not available in specified region
- Endpoint configuration incorrect

**Solutions**:
- Explicitly set the `Region` property in health check options
- Verify service availability in target region using AWS Console
- Check AWS service endpoint URLs for the region
- Ensure region format matches AWS standards (e.g., `us-east-1`)

### IAM Permission Denied

**Symptom**: Health check returns `Unhealthy` with "Access Denied" or "not authorized" errors.

**Possible Causes**:
- IAM policy missing required permissions
- Resource-based policy denying access
- Service Control Policies (SCPs) blocking access
- Incorrect resource ARN in policy

**Solutions**:
- Review IAM policy and add required service permissions (e.g., `dynamodb:DescribeTable`, `s3:GetBucketLocation`)
- Check resource-based policies on target resources
- Verify SCPs in AWS Organizations allow the action
- Use IAM Policy Simulator to test permissions
- Check CloudTrail logs for detailed authorization failures

### Resource Not Found

**Symptom**: Health check fails with "resource not found" or does not exist errors.

**Possible Causes**:
- Resource name incorrect or typo
- Resource in different AWS region
- Resource deleted or not yet created
- Case sensitivity in resource names

**Solutions**:
- Verify exact resource names using AWS Console or AWS CLI
- Check resource exists in the configured region
- Validate resource naming follows AWS conventions
- Ensure resource is fully created before health check runs

### Configuration Not Found

**Symptom**: `InvalidOperationException` during startup with "Configuration for health check '<name>' not found" message.

**Possible Causes**:
- Configuration section missing from `appsettings.json`
- Mismatch between health check name and configuration section name
- Typos in configuration keys
- Wrong configuration file loaded

**Solutions**:
- Ensure configuration section exists in `appsettings.json`
- Verify the name used in `AddAWS<Service>("<name>")` matches the configuration section name
- Check for typos in configuration keys (case-sensitive)
- Verify correct `appsettings.json` file is being loaded for the environment

