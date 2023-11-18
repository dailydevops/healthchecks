# HealthChecks

![GitHub](https://img.shields.io/github/license/dailydevops/healthchecks?logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/dailydevops/healthchecks?logo=github)
![GitHub repo size](https://img.shields.io/github/repo-size/dailydevops/healthchecks?logo=github)
[![GitHub Pipeline CI](https://github.com/dailydevops/healthchecks/actions/workflows/cicd.yml/badge.svg?branch=main&event=push)](https://github.com/dailydevops/healthchecks/actions/workflows/cicd.yml)

![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/dailydevops/healthchecks/main?logo=codefactor)
![Codecov](https://img.shields.io/codecov/c/github/dailydevops/healthchecks?logo=codecov)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=dailydevops_healthchecks&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=dailydevops_healthchecks)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=dailydevops_healthchecks&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=dailydevops_healthchecks)

## What is this repository about?
This is a mono repository for several NuGet packages based on the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks) package. The main goal of this repository is to provide a set of health checks for different services and frameworks, which are fully configurable either via code or configuration.

### What is the difference between this repository and the [AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) repository?
The main difference is that we try to focus on providing packages that are fully configurable via code or configuration. This means that you can configure the health checks in your `Program.cs` file, or in your `appsettings.json` file, or in any other configuration provider. In some cases, we provide the same healthcheck for a service with an alternative implementation. For example, we provide a healthcheck for MySql that is based on `MySql.Data` and one that is based on `MySqlConnector`. This allows you to choose the implementation that best suits your needs or fits your existing dependencies.
In addition, we try to support the latest LTS and STS versions of .NET ([.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)) as well as the latest preview version of .NET for at least 3 years, but we **can't guarantee** this. This depends on the support of related NuGet packages and the .NET platform itself. See the [Supported .NET Version](#supported-net-version) section for more details.

## NuGet packages
The following table lists all currently available NuGet packages. For more details about the packages, please visit the corresponding NuGet page.

<!-- packages:start -->
| Package Name | Current Version | Downloads | Description |
|--------------|:----------------|:----------|-------------|
| **[NetEvolve.HealthChecks](https://www.nuget.org/packages/NetEvolve.HealthChecks/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks) | *Contains general application HealthChecks.* |
| **[NetEvolve.HealthChecks.MySql](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.MySql?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.MySql?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql) | *Contains HealthChecks for MySql, based on `MySql.Data`.* |
| **[NetEvolve.HealthChecks.MySql.Connector](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.MySql.Connector?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.MySql.Connector?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector) | *Contains HealthChecks for MySql, based on `MySqlConnector`.* |
| **[NetEvolve.HealthChecks.Npgsql](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Npgsql?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Npgsql?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql) | *Contains HealthChecks for PostgreSQL, based on `Npgsql`.* |
| **[NetEvolve.HealthChecks.Oracle](https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Oracle?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Oracle?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle) | *Contains HealthChecks for Oracle Databases.* |
| **[NetEvolve.HealthChecks.SQLite](https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SQLite?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SQLite?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite) | *Contains HealthChecks for SQLite.* |
| **[NetEvolve.HealthChecks.SqlServer](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SqlServer?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlServer?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer) | *Contains HealthChecks for Microsoft SqlServer, based on `Microsoft.Data.SqlClient`.* |
| **[NetEvolve.HealthChecks.SqlServer.Legacy](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy/)** | [![Nuget](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SqlServer.Legacy?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy) | [![Nuget](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlServer.Legacy?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy) | *Contains HealthChecks for Microsoft SqlServer, based on `System.Data.SqlClient`.* |
<!-- packages:end -->

### Package naming explanation
The package names are based on the following naming schema - `NetEvolve.HealthChecks.<GroupName?>.<ServiceName>`

The `GroupName` is optional and is used to group related services. For example, all azure platform services are grouped under `Azure`. The `ServiceName` is the name of the service for which the health check is provided. For example, `SqlServer` or `MySql`.

The following table lists all planned and used groups. We will add more groups maybe in the future, if the demand is there.
- Apache
- AWS
- Azure
- GCP

## Supported .NET version
We try to support the latest LTS and STS versions of .NET ([.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)), as well as the latest preview version of .NET. We will try to support each framework version for at least 3 years, but we can't guarantee it. This depends on the support of related NuGet packages and the .NET platform itself.

| .NET Version |       Supported        |              Until               |
|--------------|:----------------------:|:--------------------------------:|
| .NET 6.0     | :heavy_check_mark: Yes | :exclamation: Dec. 2024          |
| .NET 7.0     | :heavy_check_mark: Yes |                                  |
| .NET 8.0     | :heavy_check_mark: Yes |                                  |

Why did we choose this approach? Because we want to be able to take advantage of the latest language features of the .NET platform and the performance gains that come with them. We know that not all of our NuGet packages will gain performance from this, but this is our general strategy and nobody knows what the future will bring.

### Where can I find more information about the end-of-life (EOL) date for the relevant components?
To get more information about the end-of-life (EOL) date for the relevant components, please visit the website of the creators of the components or try the website [endoflife.date](https://endoflife.date/).

## Why not .NET Standard?
With the .NET Standard Microsoft created a specification for APIs that are intended to be available on all .NET implementations. This was a great idea, but it also has some drawbacks. The main drawback is that the .NET Standard is a specification and not an implementation. This means that the real work is done by .NET implementations, such as .NET 5.0 and later versions. Which is why we decided us against the .NET Standard and for the concrete .NET implementations.

See [The future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/) for more details.
