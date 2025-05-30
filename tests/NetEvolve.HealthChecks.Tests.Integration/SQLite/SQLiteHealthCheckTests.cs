namespace NetEvolve.HealthChecks.Tests.Integration.SQLite;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.SQLite;
using Xunit;

[TestGroup(nameof(SQLite))]
public class SQLiteHealthCheckTests : HealthCheckTestBase
{
    private const string ConnectionString = "Data Source=:memory:";

    [Fact]
    public async Task AddSQLite_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSQLite(
                    "TestContainerHealthy",
                    options => options.ConnectionString = ConnectionString
                );
            },
            HealthStatus.Healthy
        );

    [Fact]
    public async Task AddSQLite_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(
                    healthChecks => healthChecks.AddSQLite("TestContainerHealthy").AddSQLite("TestContainerHealthy"),
                    HealthStatus.Healthy
                );
            }
        );

    [Fact]
    public async Task AddSQLite_UseOptions_ShouldReturnDegraded() =>
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

    [Fact]
    public async Task AddSQLite_UseOptions_ShouldReturnUnhealthy() =>
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

    [Fact]
    public async Task AddSQLite_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSQLite("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestContainerHealthy:ConnectionString", ConnectionString },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSQLite_UseConfiguration_ShouldReturnDegraded() =>
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

    [Fact]
    public async Task AddSQLite_UseConfigration_ConnectionStringEmpty_ThrowException() =>
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

    [Fact]
    public async Task AddSQLite_UseConfigration_TimeoutMinusTwo_ThrowException() =>
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
