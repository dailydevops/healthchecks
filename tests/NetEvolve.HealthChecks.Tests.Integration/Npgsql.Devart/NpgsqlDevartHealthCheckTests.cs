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
[ClassDataSource<NpgsqlDatabase>(Shared = InstanceSharedType.PostgreSql)]
[RunOn(OS.Windows)]
[Skip("Devart.Data.PostgreSql requires a license.")]
public class NpgsqlDevartHealthCheckTests : HealthCheckTestBase
{
    private readonly NpgsqlDatabase _database;

    public NpgsqlDevartHealthCheckTests(NpgsqlDatabase database) => _database = database;

    [Test]
    public async Task AddPostgreSqlDevart_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPostgreSqlDevart(
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
    public async Task AddPostgreSqlDevart_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPostgreSqlDevart(
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
    public async Task AddPostgreSqlDevart_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSqlDevart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:PostgreSql:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:PostgreSql:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddPostgreSqlDevart_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSqlDevart("TestContainerDegraded"),
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
    public async Task AddPostgreSqlDevart_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSqlDevart("TestNoValues"),
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
    public async Task AddPostgreSqlDevart_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPostgreSqlDevart("TestNoValues"),
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

    [Test]
    public async Task AddPostgreSqlDevart_UseOptions_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPostgreSqlDevart(
                    "TestNoValues",
                    options => options.ConnectionString = ""
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddPostgreSqlDevart_UseOptions_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPostgreSqlDevart(
                    "TestNoValues",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Timeout = -2;
                    }
                );
            },
            HealthStatus.Unhealthy
        );
}