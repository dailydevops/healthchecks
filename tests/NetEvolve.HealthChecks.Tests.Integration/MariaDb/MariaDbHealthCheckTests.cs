namespace NetEvolve.HealthChecks.Tests.Integration.MariaDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MariaDb;

[TestGroup(nameof(MariaDb))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<MariaDbDatabase>(Shared = SharedType.PerClass)]
public class MariaDbHealthCheckTests : HealthCheckTestBase
{
    private readonly MariaDbDatabase _database;

    public MariaDbHealthCheckTests(MariaDbDatabase database) => _database = database;

    [Test]
    public async Task AddMariaDb_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMariaDb(
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
    public async Task AddMariaDb_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMariaDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1; DO SLEEP(1);";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddMariaDb_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMariaDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SIGNAL SQLSTATE '45001' SET MESSAGE_TEXT = 'This is a test.';";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddMariaDb_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMariaDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MariaDb:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MariaDb:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMariaDb_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMariaDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MariaDb:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MariaDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMariaDb_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMariaDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MariaDb:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMariaDb_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMariaDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MariaDb:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MariaDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
