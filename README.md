# HealthChecks

![GitHub](https://img.shields.io/github/license/dailydevops/healthchecks?logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/dailydevops/healthchecks?logo=github)
![GitHub repo size](https://img.shields.io/github/repo-size/dailydevops/healthchecks?logo=github)
[![GitHub Pipeline CI](https://github.com/dailydevops/healthchecks/actions/workflows/cicd.yml/badge.svg?branch=main&event=push)](https://github.com/dailydevops/healthchecks/actions/workflows/cicd.yml)

[![CodeFactor](https://www.codefactor.io/repository/github/dailydevops/healthchecks/badge)](https://www.codefactor.io/repository/github/dailydevops/healthchecks)
[![codecov](https://codecov.io/gh/dailydevops/healthchecks/graph/badge.svg?token=IHV7ZTYZLY)](https://codecov.io/gh/dailydevops/healthchecks)

## What is this repository about?
This is a mono repository for several NuGet packages based on the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks) package. The main goal of this repository is to provide a set of health checks for different services and frameworks, which are fully configurable either via code or configuration.

### What is the difference between this repository and the [AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) repository?
The main difference is that we try to focus on providing packages that are fully configurable via code or configuration. This means that you can configure the health checks in your `Program.cs` file, or in your `appsettings.json` file, or in any other configuration provider. In some cases, we provide the same healthcheck for a service with an alternative implementation. For example, we provide a healthcheck for MySql that is based on `MySql.Data` and one that is based on `MySqlConnector`. This allows you to choose the implementation that best suits your needs or fits your existing dependencies.

In addition, we try to support the latest LTS and STS versions of .NET ([.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)) as well as the latest preview version of .NET for at least 3 years, but we **can't guarantee** this. This depends on the support of related NuGet packages and the .NET platform itself. See the [Supported .NET Version](#supported-net-version) section for more details.

## NuGet packages
The following table lists all currently available NuGet packages. For more details about the packages, please visit the corresponding NuGet page.

<!-- packages:start -->
| Package Name | NuGet Link      |
|:-------------|:---------------:|
| [NetEvolve.HealthChecks](https://www.nuget.org/packages/NetEvolve.HealthChecks/) <br/><small>Contains general application HealthChecks.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks/#readme-body-tab) |
| [NetEvolve.HealthChecks.Abstractions](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/) <br/><small>Contains abstract implementations for the `NetEvolve.HealthChecks`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Abstractions?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/#readme-body-tab) |
| [NetEvolve.HealthChecks.Apache.ActiveMq](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.ActiveMq/) <br/><small>Contains HealthChecks for Apache ActiveMq, based on the NuGet package `Apache.NMS.ActiveMQ`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Apache.ActiveMq?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.ActiveMq/#readme-body-tab) |
| [NetEvolve.HealthChecks.Apache.Kafka](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Kafka/) <br/><small>Contains HealthChecks for Apache Kafka, based on the NuGet package `Confluent.Kafka`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Apache.Kafka?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Apache.Kafka/#readme-body-tab) |
| [NetEvolve.HealthChecks.AWS](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS/) <br/><small>Contains HealthChecks for various AWS services.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS/#readme-body-tab) |
| [NetEvolve.HealthChecks.AWS.SNS](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SNS/) <br/><small>Contains HealthChecks for AWS Simple Notification Service (SNS).</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.AWS.SNS?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.AWS.SNS/#readme-body-tab) |
| [NetEvolve.HealthChecks.Azure](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure/) <br/><small>Contains HealthChecks for various Azure services.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure/#readme-body-tab) |
| [NetEvolve.HealthChecks.Azure.Blobs](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/) <br/><small>Contains HealthChecks for Azure Blob Storage.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Blobs?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Blobs/#readme-body-tab) |
| [NetEvolve.HealthChecks.Azure.Queues](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/) <br/><small>Contains HealthChecks for Azure Queue Storage.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Queues?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Queues/#readme-body-tab) |
| [NetEvolve.HealthChecks.Azure.ServiceBus](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ServiceBus/) <br/><small>Contains HealthChecks for Azure Service Bus.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.ServiceBus?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.ServiceBus/#readme-body-tab) |
| [NetEvolve.HealthChecks.Azure.Tables](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/) <br/><small>Contains HealthChecks for Azure Table Storage.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.Tables?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.Tables/#readme-body-tab) |
| [NetEvolve.HealthChecks.ClickHouse](https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse/) <br/><small>Contains HealthChecks for ClickHouse, based on the nuget package `ClickHouse.Client`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.ClickHouse?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.ClickHouse/#readme-body-tab) |
| [NetEvolve.HealthChecks.Dapr](https://www.nuget.org/packages/NetEvolve.HealthChecks.Dapr/) <br/><small>Contains HealthChecks for Dapr, based on the nuget package `Dapr.Client`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Dapr?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Dapr/#readme-body-tab) |
| [NetEvolve.HealthChecks.DB2](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2/) <br/><small>Contains HealthChecks for Db2, based on the nuget packages `Net.IBM.Data.Db2` (Windows), `Net.IBM.Data.Db2-lnx` (Linux) and `Net.IBM.Data.Db2-osx` (OSX).</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.DB2?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.DB2/#readme-body-tab) |
| [NetEvolve.HealthChecks.DuckDB](https://www.nuget.org/packages/NetEvolve.HealthChecks.DuckDB/) <br/><small>Contains HealthChecks for DuckDB, based on the nuget package `DuckDB.NET.Data`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.DuckDB?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.DuckDB/#readme-body-tab) |
| [NetEvolve.HealthChecks.Firebird](https://www.nuget.org/packages/NetEvolve.HealthChecks.Firebird/) <br/><small>Contains HealthChecks for Firebird, based on the nuget package `FirebirdSql.Data.FirebirdClient`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Firebird?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Firebird/#readme-body-tab) |
| [NetEvolve.HealthChecks.MongoDb](https://www.nuget.org/packages/NetEvolve.HealthChecks.MongoDb/) <br/><small>Contains HealthChecks for MongoDb, based on the nuget package `MongoDB.Driver`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.MongoDb?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.MongoDb/#readme-body-tab) |
| [NetEvolve.HealthChecks.MySql](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql/) <br/><small>Contains HealthChecks for MySql, based on the nuget package `MySql.Data`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.MySql?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql/#readme-body-tab) |
| [NetEvolve.HealthChecks.MySql.Connector](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector/) <br/><small>Contains HealthChecks for MySql, based on the nuget package `MySqlConnector`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.MySql.Connector?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.MySql.Connector/#readme-body-tab) |
| [NetEvolve.HealthChecks.Npgsql](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql/) <br/><small>Contains HealthChecks for PostgreSQL, based on the nuget package `Npgsql`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Npgsql?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Npgsql/#readme-body-tab) |
| [NetEvolve.HealthChecks.Odbc](https://www.nuget.org/packages/NetEvolve.HealthChecks.Odbc/) <br/><small>Contains HealthChecks for ODBC data sources, based on the nuget package `System.Data.Odbc`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Odbc?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Odbc/#readme-body-tab) |
| [NetEvolve.HealthChecks.Oracle](https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle/) <br/><small>Contains HealthChecks for Oracle Databases, based on the nuget package `Oracle.ManagedDataAccess.Core`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Oracle?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Oracle/#readme-body-tab) |
| [NetEvolve.HealthChecks.Qdrant](https://www.nuget.org/packages/NetEvolve.HealthChecks.Qdrant/) <br/><small>Contains HealthChecks for Qdrant Vector database, based on the nuget package `Qdrant.Client`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Qdrant?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Qdrant/#readme-body-tab) |
| [NetEvolve.HealthChecks.RabbitMQ](https://www.nuget.org/packages/NetEvolve.HealthChecks.RabbitMQ/) <br/><small>Contains HealthChecks for RabbitMQ, based on the nuget package `RabbitMQ.Client`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.RabbitMQ?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.RabbitMQ/#readme-body-tab) |
| [NetEvolve.HealthChecks.RavenDb](https://www.nuget.org/packages/NetEvolve.HealthChecks.RavenDb/) <br/><small>Contains HealthChecks for RavenDb, based on the nuget package `RavenDB.Client`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.RavenDb?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.RavenDb/#readme-body-tab) |
| [NetEvolve.HealthChecks.Redis](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redis/) <br/><small>Contains HealthChecks for Redis, based on the NuGet package `StackExchange.Redis`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Redis?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redis/#readme-body-tab) |
| [NetEvolve.HealthChecks.Redpanda](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redpanda/) <br/><small>Contains HealthChecks for Redpanda, based on the NuGet package `Confluent.Kafka`. This is a temporary measure; if a dedicated Redpanda client is provided in the future, we will use it immediately.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Redpanda?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Redpanda/#readme-body-tab) |
| [NetEvolve.HealthChecks.SqlEdge](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlEdge/) ❌ **DEPRECATED**<br/><small>Contains HealthChecks for SqlEdge, based on the nuget package `Microsoft.Data.SqlClient`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlEdge?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlEdge/#readme-body-tab) |
| [NetEvolve.HealthChecks.SQLite](https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite/) <br/><small>Contains HealthChecks for SQLite, based on the nuget package `Microsoft.Data.Sqlite`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SQLite?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite/#readme-body-tab) |
| [NetEvolve.HealthChecks.SQLite.Legacy](https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite.Legacy/) <br/><small>Contains HealthChecks for SQLite, based on the nuget package `System.Data.Sqlite`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SQLite.Legacy?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SQLite.Legacy/#readme-body-tab) |
| [NetEvolve.HealthChecks.SqlServer](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer/) <br/><small>Contains HealthChecks for Microsoft SqlServer, based on the nuget package `Microsoft.Data.SqlClient`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlServer?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer/#readme-body-tab) |
| [NetEvolve.HealthChecks.SqlServer.Legacy](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy/) <br/><small>Contains HealthChecks for Microsoft SqlServer, based on the nuget package `System.Data.SqlClient`.</small> | [![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.SqlServer.Legacy?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/NetEvolve.HealthChecks.SqlServer.Legacy/#readme-body-tab) |
<!-- packages:end -->

## Package naming explanation
The package names are based on the following naming schema - `NetEvolve.HealthChecks.<ServiceGroup?>.<ServiceName>.<ServiceVersion?>`

### Explanation of the naming schema:

- `NetEvolve` is the name of the organization that maintains this repository.
- `HealthChecks` indicates that this package contains health checks for various services.
- `<ServiceGroup?>` is an optional part that groups related services together. It helps to categorize the health checks based on the service provider or technology.
- `<ServiceName>` is the name of the service for which the health check is provided. It specifies the actual service that the health check is targeting.
- `<ServiceVersion?>` is an optional suffix that identifies a specific implementation or version of the client library used to connect to the service. This is useful when there are multiple client libraries available for the same service, each with different implementations or approaches.

#### ServiceGroup and ServiceName

The `ServiceGroup` is optional and is used to group related services. For example, all azure platform services are grouped under `Azure`. The `ServiceName` is the name of the service for which the health check is provided. For example, `SqlServer` or `MySql`.

The following table lists all planned and used groups. We will add more groups maybe in the future, if the demand is there.
- Apache
- AWS
- Azure
- GCP

#### ServiceVersion

The `ServiceVersion` in the package naming schema `NetEvolve.HealthChecks.<ServiceGroup?>.<ServiceName>.<ServiceVersion?>` refers to an optional suffix that identifies a specific implementation or version of the client library used to connect to the service.

This component is used when:

1.	Multiple Client Libraries: When there are multiple client libraries available for the same service, each with different implementations or approaches.
2.	Legacy vs Modern Implementations: To distinguish between older `Legacy` and newer implementations of a client library for the same service.

This naming convention allows developers to choose the specific client implementation that best matches their existing dependencies without having to change their application architecture. For example, if your application already uses `MySqlConnector`, you would choose the corresponding health check package that uses the same client library.

The `ServiceVersion` is particularly valuable in scenarios where:
- Different client libraries have varying features, performance characteristics, or compatibility.
- You need to maintain compatibility with specific versions of a service.

### Examples in the Repository:      

1. SQL Server Client Libraries:
    - `NetEvolve.HealthChecks.SqlServer` - Uses the modern `Microsoft.Data.SqlClient`
    - `NetEvolve.HealthChecks.SqlServer.Legacy` - Uses the legacy `System.Data.SqlClient`

2. MySQL Client Libraries:
    - `NetEvolve.HealthChecks.MySql` - Uses the implementation of `MySql.Data`
    - `NetEvolve.HealthChecks.MySql.Connector` - Uses the alternative implementation of `MySqlConnector`

3. RabbitMQ Client Libraries:
    - `NetEvolve.HealthChecks.RabbitMQ` - Uses the latest version of `RabbitMQ.Client`
    - `NetEvolve.HealthChecks.RabbitMQ.V6` - Uses the older version of `RabbitMQ.Client` (version 6.x)

## Supported .NET version
We try to support the LTS and STS versions of .NET ([.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)), as well as the latest preview version of .NET. We will try to support each framework version for at least 3 years, but we can't guarantee it. This depends on the support of related NuGet packages and the .NET platform itself.

| .NET Version                     | Supported                                                        |
|----------------------------------|:-----------------------------------------------------------------|
| **.NET Standard**                | :x: No                                                           |
| **.NET 7.0 or earlier versions** | :x: No                                                           |
| **.NET 8.0**                     | :white_check_mark: Yes                                           |
| **.NET 9.0**                     | :white_check_mark: Yes                                           |
| **.NET 10.0**                    | :white_square_button: Early stage of planning, not yet supported |

Why did we choose this approach? Because we want to be able to take advantage of the latest language features of the .NET platform and the performance gains that come with them. We know that not all of our NuGet packages will gain performance from this, but this is our general strategy and nobody knows what the future will bring.

### Where can I find more information about the end-of-life (EOL) date for the relevant components?
To get more information about the end-of-life (EOL) date for the relevant components, please visit the website of the creators of the components or try the website [endoflife.date](https://endoflife.date/).

## Why not .NET Standard?
With the .NET Standard Microsoft created a specification for APIs that are intended to be available on all .NET implementations. This was a great idea, but it also has some drawbacks. The main drawback is that the .NET Standard is a specification and not an implementation. This means that the real work is done by .NET implementations, such as .NET 5.0 and later versions. Which is why we decided us against the .NET Standard and for the concrete .NET implementations.

See [The future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/) for more details.
