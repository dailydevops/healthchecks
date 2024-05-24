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
<table>
  <thead>
    <tr>
      <td><b>Package Name</b></td>
      <td><b>Current Version</b></td>
      <td><b>Downloads</b></td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks/"><b>NetEvolve.HealthChecks</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains general application HealthChecks.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/"><b>NetEvolve.HealthChecks.Abstractions</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Abstractions?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Abstractions?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains abstract implementations for the `NetEvolve.HealthChecks`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Kafka/"><b>NetEvolve.HealthChecks.Apache.Kafka</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Kafka/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Apache.Kafka?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Kafka/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Apache.Kafka?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Apache Kafka, based on the NuGet package `Confluent.Kafka`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/"><b>NetEvolve.HealthChecks.Azure.Blobs</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Blobs?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Blobs?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Azure Blob Storage.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/"><b>NetEvolve.HealthChecks.Azure.Queues</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Queues?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Queues?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Azure Queue Storage.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/"><b>NetEvolve.HealthChecks.Azure.Tables</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.Tables?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Tables?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Azure Table Storage.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse/"><b>NetEvolve.HealthChecks.ClickHouse</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.ClickHouse?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.ClickHouse?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for ClickHouse, based on the nuget package `ClickHouse.Client`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Dapr/"><b>NetEvolve.HealthChecks.Dapr</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Dapr/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Dapr?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Dapr/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Dapr?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Dapr, based on the nuget package `Dapr.Client`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql/"><b>NetEvolve.HealthChecks.MySql</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.MySql?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.MySql?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for MySql, based on the nuget package `MySql.Data`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector/"><b>NetEvolve.HealthChecks.MySql.Connector</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.MySql.Connector?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.MySql.Connector?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for MySql, based on the nuget package `MySqlConnector`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql/"><b>NetEvolve.HealthChecks.Npgsql</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Npgsql?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Npgsql?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for PostgreSQL, based on the nuget package `Npgsql`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle/"><b>NetEvolve.HealthChecks.Oracle</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Oracle?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Oracle?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Oracle Databases, based on the nuget package `Oracle.ManagedDataAccess.Core`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Redis/"><b>NetEvolve.HealthChecks.Redis</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Redis/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Redis?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Redis/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Redis?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Redis, based on the NuGet package `StackExchange.Redis`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Redpanda/"><b>NetEvolve.HealthChecks.Redpanda</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Redpanda/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Redpanda?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.Redpanda/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Redpanda?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Redpanda, based on the NuGet package `Confluent.Kafka`. This is a temporary measure; if a dedicated Redpanda client is provided in the future, we will use it immediately.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlEdge/"><b>NetEvolve.HealthChecks.SqlEdge</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlEdge/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SqlEdge?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlEdge/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlEdge?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for SqlEdge, based on the nuget package `Microsoft.Data.SqlClient`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite/"><b>NetEvolve.HealthChecks.SQLite</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SQLite?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SQLite?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for SQLite, based on the nuget package `Microsoft.Data.Sqlite`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer/"><b>NetEvolve.HealthChecks.SqlServer</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SqlServer?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlServer?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Microsoft SqlServer, based on the nuget package `Microsoft.Data.SqlClient`.</td>
    </tr>
    <tr>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy/"><b>NetEvolve.HealthChecks.SqlServer.Legacy</b></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy/"><img src="https://img.shields.io/nuget/v/NetEvolve.HealthChecks.SqlServer.Legacy?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy/"><img src="https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlServer.Legacy?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3>Contains HealthChecks for Microsoft SqlServer, based on the nuget package `System.Data.SqlClient`.</td>
    </tr>
  </tbody>
</table>
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
We try to support the LTS and STS versions of .NET ([.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)), as well as the latest preview version of .NET. We will try to support each framework version for at least 3 years, but we can't guarantee it. This depends on the support of related NuGet packages and the .NET platform itself.

| .NET Version                     | Supported                                               |
|----------------------------------|:--------------------------------------------------------|
| **.NET Standard**                | :x: No                                                  |
| **.NET 5.0 or earlier versions** | :x: No                                                  |
| **.NET 6.0**                     | :white_check_mark: Yes, until Dec. 2024                 |
| **.NET 7.0**                     | :white_check_mark: Yes, until Jun. 2024 at the earliest |
| **.NET 8.0**                     | :white_check_mark: Yes                                  |
| **.NET 9.0**                     | :x: No, until Apr. 2024 at the earliest                 |



Why did we choose this approach? Because we want to be able to take advantage of the latest language features of the .NET platform and the performance gains that come with them. We know that not all of our NuGet packages will gain performance from this, but this is our general strategy and nobody knows what the future will bring.

### Where can I find more information about the end-of-life (EOL) date for the relevant components?
To get more information about the end-of-life (EOL) date for the relevant components, please visit the website of the creators of the components or try the website [endoflife.date](https://endoflife.date/).

## Why not .NET Standard?
With the .NET Standard Microsoft created a specification for APIs that are intended to be available on all .NET implementations. This was a great idea, but it also has some drawbacks. The main drawback is that the .NET Standard is a specification and not an implementation. This means that the real work is done by .NET implementations, such as .NET 5.0 and later versions. Which is why we decided us against the .NET Standard and for the concrete .NET implementations.

See [The future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/) for more details.
