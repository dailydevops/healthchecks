# NetEvolve.HealthChecks.Minio

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Minio?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Minio/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Minio?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Minio/)

This package provides a health check for Minio, based on the [AWS SDK for .NET](https://www.nuget.org/packages/AWSSDK.S3/) package.
The main purpose is to check that the Minio service is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Minio
```

## Health Check - Minio Liveness
The health check is a liveness check. It will check that the Minio service is reachable and that the client can connect to it.
If the service needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Minio` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Minio;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `minio`, `s3` and `storage` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Minio` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddMinio("<name>");
```

The configuration looks like this:
```json
{
    ..., // other configuration
    "HealthChecks": {
        "Minio": {
            "<name>": {
                "ServiceUrl": "<minio-service-url>", // required, e.g., "http://localhost:9000"
                "AccessKey": "<access-key>", // required
                "SecretKey": "<secret-key>", // required
                "BucketName": "<bucket-name>", // required
                "Mode": "BasicAuthentication", // required for authentication
                "Timeout": "<timeout>" // optional, default is 100 milliseconds
            }
        }
    }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one Minio instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddMinio("<name>", options =>
{
        options.ServiceUrl = "<minio-service-url>"; // required, e.g., "http://localhost:9000"
        options.AccessKey = "<access-key>"; // required
        options.SecretKey = "<secret-key>"; // required
        options.BucketName = "<bucket-name>"; // required
        options.Mode = CreationMode.BasicAuthentication; // required for authentication
        options.Timeout = "<timeout>"; // optional, default is 100 milliseconds
        ... // other configuration
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddMinio("<name>", options => ..., "minio", "object-storage");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
