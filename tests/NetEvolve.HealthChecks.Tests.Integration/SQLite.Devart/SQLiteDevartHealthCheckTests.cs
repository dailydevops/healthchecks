namespace NetEvolve.HealthChecks.Tests.Integration.SQLite.Devart;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SQLite.Devart;

[TestGroup($"{nameof(SQLite)}.{nameof(Devart)}")]
[TestGroup("Z01TestGroup")]
[Skip("Devart.SQLite requires a license.")]
public class SQLiteDevartHealthCheckTests : HealthCheckTestBase
{
    private readonly string _databasePath;

    public SQLiteDevartHealthCheckTests() =>
        _databasePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");

    [Test]
    public async Task AddSQLiteDevart_UseOptions_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLiteDevart(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = $"Data Source={_databasePath}";
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSQLiteDevart_UseOptions_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLiteDevart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = $"Data Source={_databasePath}";
                        options.Command = "SELECT 1;";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSQLiteDevart_UseOptions_Unhealthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLiteDevart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = $"Data Source={_databasePath}";
                        options.Command = "SELECT * FROM NonExistentTable;";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLiteDevart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestContainerHealthy:ConnectionString", $"Data Source={_databasePath}" },
                    { "HealthChecks:SQLite:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLiteDevart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestContainerDegraded:ConnectionString", $"Data Source={_databasePath}" },
                    { "HealthChecks:SQLite:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_ConnectionStringEmpty_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLiteDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLiteDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestNoValues:ConnectionString", $"Data Source={_databasePath}" },
                    { "HealthChecks:SQLite:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );
}
