namespace NetEvolve.HealthChecks.Tests.Integration.DB2.Devart;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.DB2.Devart;
using TUnit.Core.Enums;

[TestGroup($"{nameof(DB2)}.{nameof(Devart)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<DB2Database>(Shared = InstanceSharedType.DB2)]
[RunOn(OS.Windows)]
[Skip("Devart.DB2 requires a license.")]
public class DB2DevartHealthCheckTests : HealthCheckTestBase
{
    private readonly DB2Database _database;

    public DB2DevartHealthCheckTests(DB2Database database) => _database = database;

    [Test]
    public async Task AddDB2Devart_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDB2Devart(
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
    public async Task AddDB2Devart_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDB2Devart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddDB2Devart_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDB2Devart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT * FROM INVALID_TABLE;";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddDB2Devart_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2Devart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DB2:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:DB2:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDB2Devart_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2Devart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DB2:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:DB2:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDB2Devart_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2Devart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DB2:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDB2Devart_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2Devart("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DB2:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:DB2:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
