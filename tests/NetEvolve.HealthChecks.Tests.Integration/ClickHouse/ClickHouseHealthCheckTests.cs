namespace NetEvolve.HealthChecks.Tests.Integration.ClickHouse;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.ClickHouse;

[TestGroup(nameof(ClickHouse))]
[ClassDataSource<ClickHouseDatabase>(Shared = SharedType.PerTestSession)]
public class ClickHouseHealthCheckTests : HealthCheckTestBase
{
    private readonly ClickHouseDatabase _database;

    public ClickHouseHealthCheckTests(ClickHouseDatabase database) => _database = database;

    [Test]
    public async Task AddClickHouse_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddClickHouse_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse(
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
    public async Task AddClickHouse_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "Error";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddClickHouse_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddClickHouse("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ClickHouse:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:ClickHouse:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddClickHouse_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddClickHouse("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ClickHouse:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:ClickHouse:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddClickHouse_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddClickHouse("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ClickHouse:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddClickHouse_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddClickHouse("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ClickHouse:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:ClickHouse:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
