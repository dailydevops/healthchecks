namespace NetEvolve.HealthChecks.Tests.Integration.SQLite;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SQLite;

[TestGroup(nameof(SQLite))]
[TestGroup("Z01TestGroup")]
public class SQLiteHealthCheckTests : HealthCheckTestBase
{
    private const string ConnectionString = "Data Source=:memory:";

    [Test]
    public async Task AddSQLite_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLite(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = ConnectionString;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddSQLite_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLite(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = ConnectionString;
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddSQLite_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLite(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = ConnectionString;
                        options.Command = "SELECT 1 = `1`";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddSQLite_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLite("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestContainerHealthy:ConnectionString", ConnectionString },
                    { "HealthChecks:SQLite:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSQLite_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLite("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestContainerDegraded:ConnectionString", ConnectionString },
                    { "HealthChecks:SQLite:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSQLite_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLite("TestNoValues"),
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
    public async Task AddSQLite_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLite("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestNoValues:ConnectionString", ConnectionString },
                    { "HealthChecks:SQLite:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
