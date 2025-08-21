namespace NetEvolve.HealthChecks.Tests.Integration.SQLite.Devart;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SQLite.Devart;

[TestGroup($"{nameof(SQLite)}.{nameof(Devart)}")]
[ClassDataSource<SqliteDatabase>(Shared = InstanceSharedType.SQLite)]
[Skip("Devart.Data.SQLite requires a license.")]
public class SQLiteDevartHealthCheckTests : HealthCheckTestBase
{
    private readonly SqliteDatabase _database;

    public SQLiteDevartHealthCheckTests(SqliteDatabase database) => _database = database;

    [Test]
    public async Task AddSQLiteDevart_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLiteDevart(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddSQLiteDevart_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLiteDevart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1;";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddSQLiteDevart_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLiteDevart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT invalid_syntax;";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLiteDevart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SQLite:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLiteDevart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SQLite:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
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
            }
        );

    [Test]
    public async Task AddSQLiteDevart_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLiteDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SQLite:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}