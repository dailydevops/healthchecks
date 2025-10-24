# NetEvolve.HealthChecks.Abstractions

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Abstractions?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Abstractions?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/)

This package provides a set of abstractions for creating health checks in .NET applications. It is based on the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks/) package and used for creating the NetEvolve.HealthChecks libraries.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Abstractions
```

## Related Packages

### Packages Using These Abstractions
This package provides the base abstractions used by all NetEvolve.HealthChecks packages. For specific health check implementations, see:

#### Database Health Checks
- <a>`NetEvolve.HealthChecks.SqlServer`</a> - SQL Server health checks
- <a>`NetEvolve.HealthChecks.MySql`</a> - MySQL health checks
- <a>`NetEvolve.HealthChecks.SQLite`</a> - SQLite health checks
- <a>`NetEvolve.HealthChecks.Npgsql`</a> - PostgreSQL health checks
- <a>`NetEvolve.HealthChecks.Oracle`</a> - Oracle health checks
- <a>`NetEvolve.HealthChecks.MongoDb`</a> - MongoDB health checks

#### Cloud Service Health Checks
- <a>`NetEvolve.HealthChecks.AWS`</a> - AWS service health checks bundle
- <a>`NetEvolve.HealthChecks.Azure`</a> - Azure service health checks bundle

#### Other Health Checks
- <a>`NetEvolve.HealthChecks.Redis`</a> - Redis health checks
- <a>`NetEvolve.HealthChecks.RabbitMQ`</a> - RabbitMQ health checks
- <a>`NetEvolve.HealthChecks.Http`</a> - HTTP endpoint health checks

For a complete list, see the [repository](https://github.com/dailydevops/healthchecks).