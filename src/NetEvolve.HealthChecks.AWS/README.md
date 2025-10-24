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

