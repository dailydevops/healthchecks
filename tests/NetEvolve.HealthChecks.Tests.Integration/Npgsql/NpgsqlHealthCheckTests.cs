namespace NetEvolve.HealthChecks.Tests.Integration.Npgsql;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Npgsql;

[TestGroup(nameof(Npgsql))]
[ClassDataSource<NpgsqlDatabase>(Shared = InstanceSharedType.PostgreSql)]
public class NpgsqlHealthCheckTests : HealthCheckTestBase
{
    private readonly NpgsqlDatabase _database;

    public NpgsqlHealthCheckTests(NpgsqlDatabase database) => _database = database;

    [Test]
    public async Task AddPostgreSql_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPostgreSql(
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
    public async Task AddPostgreSql_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPostgreSql(
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
    public async Task AddPostgreSql_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPostgreSql(
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
    public async Task AddPostgreSql_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSql("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:PostgreSql:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:PostgreSql:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddPostgreSql_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSql("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:PostgreSql:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:PostgreSql:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddPostgreSql_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSql("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:PostgreSql:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddPostgreSql_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSql("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:PostgreSql:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:PostgreSql:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
