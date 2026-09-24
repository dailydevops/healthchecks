namespace NetEvolve.HealthChecks.Tests.Integration.SqlServer.Devart;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SqlServer.Devart;
using TUnit.Core.Enums;

[TestGroup($"{nameof(SqlServer)}.{nameof(Devart)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<SqlServerDatabase>(Shared = SharedType.PerClass)]
[RunOn(OS.Windows)]
[Skip("Devart.SqlServer requires a license.")]
public class SqlServerDevartHealthCheckTests : HealthCheckTestBase
{
    private readonly SqlServerDatabase _database;

    public SqlServerDevartHealthCheckTests(SqlServerDatabase database) => _database = database;

    [Test]
    public async Task AddSqlServerDevart_UseOptions_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServerDevart(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSqlServerDevart_UseOptions_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServerDevart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSqlServerDevart_UseOptions_Unhealthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServerDevart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "RAISERROR('This is a test.',16,1)";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSqlServerDevart_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerDevart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSqlServerDevart_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerDevart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSqlServerDevart_UseConfiguration_ConnectionStringEmpty_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSqlServerDevart_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );
}
