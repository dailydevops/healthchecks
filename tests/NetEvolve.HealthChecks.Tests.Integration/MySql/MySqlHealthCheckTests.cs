namespace NetEvolve.HealthChecks.Tests.Integration.MySql;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MySql;

[TestGroup(nameof(MySql))]
[ClassDataSource<MySqlDatabase>(Shared = SharedType.PerTestSession)]
public class MySqlHealthCheckTests : HealthCheckTestBase
{
    private readonly MySqlDatabase _database;

    public MySqlHealthCheckTests(MySqlDatabase database) => _database = database;

    [Test]
    public async Task AddMySql_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySql(
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
    public async Task AddMySql_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySql(
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
    public async Task AddMySql_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySql(
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
    public async Task AddMySql_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySql("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MySql:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMySql_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySql("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MySql:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMySql_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySql("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddMySql_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySql("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MySql:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
