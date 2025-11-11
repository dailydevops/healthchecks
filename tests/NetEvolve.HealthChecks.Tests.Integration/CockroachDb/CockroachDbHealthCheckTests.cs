namespace NetEvolve.HealthChecks.Tests.Integration.CockroachDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.CockroachDb;

[TestGroup(nameof(CockroachDb))]
[ClassDataSource<CockroachDbDatabase>(Shared = SharedType.PerClass)]
public class CockroachDbHealthCheckTests : HealthCheckTestBase
{
    private readonly CockroachDbDatabase _database;

    public CockroachDbHealthCheckTests(CockroachDbDatabase database) => _database = database;

    [Test]
    public async Task AddCockroachDb_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCockroachDb(
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
    public async Task AddCockroachDb_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCockroachDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddCockroachDb_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCockroachDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1 = `1`";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddCockroachDb_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCockroachDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CockroachDb:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:CockroachDb:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddCockroachDb_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCockroachDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CockroachDb:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:CockroachDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddCockroachDb_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCockroachDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CockroachDb:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddCockroachDb_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCockroachDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CockroachDb:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:CockroachDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
