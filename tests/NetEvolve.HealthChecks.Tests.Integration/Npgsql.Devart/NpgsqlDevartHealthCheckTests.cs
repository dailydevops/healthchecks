namespace NetEvolve.HealthChecks.Tests.Integration.Npgsql.Devart;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Npgsql.Devart;
using TUnit.Core.Enums;

[TestGroup($"{nameof(Npgsql)}.{nameof(Devart)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<NpgsqlDatabase>(Shared = InstanceSharedType.PostgreSql)]
[RunOn(OS.Windows)]
[Skip("Devart.PostgreSql requires a license.")]
public class NpgsqlDevartHealthCheckTests : HealthCheckTestBase
{
    private readonly NpgsqlDatabase _database;

    public NpgsqlDevartHealthCheckTests(NpgsqlDatabase database) => _database = database;

    [Test]
    public async Task AddNpgsqlDevart_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddNpgsqlDevart(
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
    public async Task AddNpgsqlDevart_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddNpgsqlDevart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1; SELECT pg_sleep(0.1);";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddNpgsqlDevart_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddNpgsqlDevart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1/0;";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddNpgsqlDevart_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNpgsqlDevart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Npgsql:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:Npgsql:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddNpgsqlDevart_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNpgsqlDevart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Npgsql:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:Npgsql:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddNpgsqlDevart_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNpgsqlDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Npgsql:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddNpgsqlDevart_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNpgsqlDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Npgsql:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:Npgsql:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
