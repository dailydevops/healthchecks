namespace NetEvolve.HealthChecks.Tests.Integration.ClickHouse;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.ClickHouse;

[TestGroup(nameof(ClickHouse))]
[TestGroup("Z01TestGroup")]
[ClassDataSource<ClickHouseDatabase>(Shared = SharedType.PerClass)]
public class ClickHouseHealthCheckTests : HealthCheckTestBase
{
    private readonly ClickHouseDatabase _database;

    public ClickHouseHealthCheckTests(ClickHouseDatabase database) => _database = database;

    [Test]
    public async Task AddClickHouse_UseOptions_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse(
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
    public async Task AddClickHouse_UseOptions_Degraded(CancellationToken cancellationToken = default) =>
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
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddClickHouse_UseOptions_Unhealthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = "Unreachable"; // Wrong connection string to simulate failure
                        options.Command = "Error";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddClickHouse_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddClickHouse("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ClickHouse:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:ClickHouse:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddClickHouse_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
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
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddClickHouse_UseConfiguration_ConnectionStringEmpty_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
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
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddClickHouse_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
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
            },
            cancellationToken: cancellationToken
        );
}
