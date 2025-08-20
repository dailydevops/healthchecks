namespace NetEvolve.HealthChecks.Tests.Integration.SqlServer;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SqlServer;

[TestGroup(nameof(SqlServer))]
[ClassDataSource<SqlServerDatabase>(Shared = InstanceSharedType.SqlServer)]
public class SqlServerHealthCheckTests : HealthCheckTestBase
{
    private readonly SqlServerDatabase _database;

    public SqlServerHealthCheckTests(SqlServerDatabase database) => _database = database;

    [Test]
    public async Task AddSqlServer_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServer(
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
    public async Task AddSqlServer_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServer(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddSqlServer_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServer(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "RAISERROR('This is a test.',16,1)";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddSqlServer_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServer("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSqlServer_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServer("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSqlServer_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServer("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSqlServer_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServer("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
