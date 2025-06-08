namespace NetEvolve.HealthChecks.Tests.Integration.Oracle;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Oracle;

[TestGroup(nameof(Oracle))]
[ClassDataSource<OracleDatabase>(Shared = SharedType.PerTestSession)]
public class OracleHealthCheckTests : HealthCheckTestBase
{
    private readonly OracleDatabase _database;

    public OracleHealthCheckTests(OracleDatabase database) => _database = database;

    [Test]
    public async Task AddOracle_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracle(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddOracle_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracle(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddOracle_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracle(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Command = "SELECT 1 = `1`;";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddOracle_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracle("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestContainerHealthy:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:Oracle:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOracle_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracle("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestContainerDegraded:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:Oracle:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOracle_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracle("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOracle_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracle("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestNoValues:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:Oracle:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
