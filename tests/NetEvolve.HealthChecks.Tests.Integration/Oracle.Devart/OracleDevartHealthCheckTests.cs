namespace NetEvolve.HealthChecks.Tests.Integration.Oracle.Devart;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Oracle.Devart;
using TUnit.Core.Enums;

[TestGroup($"{nameof(Oracle)}.{nameof(Devart)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<OracleDatabase>(Shared = SharedType.PerClass)]
[RunOn(OS.Windows)]
[Skip("Devart.Oracle requires a license.")]
public class OracleDevartHealthCheckTests : HealthCheckTestBase
{
    private readonly OracleDatabase _database;

    public OracleDevartHealthCheckTests(OracleDatabase database) => _database = database;

    [Test]
    public async Task AddOracleDevart_UseOptions_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracleDevart(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddOracleDevart_UseOptions_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracleDevart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Command = "SELECT 1 FROM dual; DBMS_LOCK.SLEEP(0.1);";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddOracleDevart_UseOptions_Unhealthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracleDevart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Command = "RAISE_APPLICATION_ERROR(-20001, 'This is a test.')";
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracleDevart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestContainerHealthy:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:Oracle:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracleDevart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestContainerDegraded:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:Oracle:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_ConnectionStringEmpty_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracleDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOracleDevart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Oracle:TestNoValues:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:Oracle:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            cancellationToken: cancellationToken
        );
}
