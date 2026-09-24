namespace NetEvolve.HealthChecks.Tests.Integration.MySql.Devart;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MySql.Devart;

[TestGroup($"{nameof(MySql)}.{nameof(Devart)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<MySqlDatabase>(Shared = SharedType.PerClass)]
[Skip("Devart.Data.MySql requires a license.")]
public class MySqlDevartHealthCheckTests : HealthCheckTestBase
{
    private readonly MySqlDatabase _database;

    public MySqlDevartHealthCheckTests(MySqlDatabase database) => _database = database;

    [Test]
    public async Task AddMySqlDevart_UseOptions_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySqlDevart(
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
    public async Task AddMySqlDevart_UseOptions_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySqlDevart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddMySqlDevart_UseOptions_Unhealthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySqlDevart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'This is a test.';";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddMySqlDevart_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySqlDevart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MySql:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddMySqlDevart_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySqlDevart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MySql:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddMySqlDevart_UseConfiguration_ConnectionStringEmpty_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySqlDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddMySqlDevart_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMySqlDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MySql:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MySql:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );
}
