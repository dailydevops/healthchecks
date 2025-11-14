namespace NetEvolve.HealthChecks.Tests.Integration.DuckDB;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.DuckDB;

[TestGroup(nameof(DuckDB))]
[TestGroup("Z01TestGroup")]
public class DuckDBHealthCheckTests : HealthCheckTestBase
{
    private const string ConnectionString = "Data Source=:memory:";

    [Test]
    public async Task AddDuckDB_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDuckDB(
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
    public async Task AddDuckDB_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDuckDB(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddDuckDB_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDuckDB(
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
    public async Task AddDuckDB_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDuckDB("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DuckDB:TestContainerHealthy:ConnectionString", ConnectionString },
                    { "HealthChecks:DuckDB:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDuckDB_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDuckDB("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DuckDB:TestContainerDegraded:ConnectionString", ConnectionString },
                    { "HealthChecks:DuckDB:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDuckDB_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDuckDB("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DuckDB:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDuckDB_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDuckDB("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DuckDB:TestNoValues:ConnectionString", ConnectionString },
                    { "HealthChecks:DuckDB:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
