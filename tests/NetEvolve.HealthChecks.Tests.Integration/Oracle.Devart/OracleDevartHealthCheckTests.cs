namespace NetEvolve.HealthChecks.Tests.Integration.Oracle.Devart;

using System;
using System.Collections.Generic;
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
    public async Task AddOracleDevart_UseOptions_Healthy() =>
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
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddOracleDevart_UseOptions_Degraded() =>
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
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddOracleDevart_UseOptions_Unhealthy() =>
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
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_Healthy() =>
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
            }
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_Degraded() =>
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
            }
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
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
            }
        );

    [Test]
    public async Task AddOracleDevart_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
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
            }
        );
}
