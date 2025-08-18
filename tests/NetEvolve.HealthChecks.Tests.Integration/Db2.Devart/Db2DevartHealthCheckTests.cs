namespace NetEvolve.HealthChecks.Tests.Integration.Db2.Devart;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Db2.Devart;
using TUnit.Core.Enums;

[TestGroup($"{nameof(Db2)}.{nameof(Devart)}")]
[ClassDataSource<Db2Database>(Shared = InstanceSharedType.DB2)]
[Skip("Devart.Data.DB2 requires a license.")]
public class Db2DevartHealthCheckTests : HealthCheckTestBase
{
    private readonly Db2Database _database;

    public Db2DevartHealthCheckTests(Db2Database database) => _database = database;

    [Test]
    public async Task AddDb2Devart_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDb2Devart(
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
    public async Task AddDb2Devart_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDb2Devart(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1 FROM SYSIBM.SYSDUMMY1; DBMS_LOCK.SLEEP(1);";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddDb2Devart_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDb2Devart(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "invalid";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddDb2Devart_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDb2Devart("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DB2:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDb2Devart_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDb2Devart("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DB2:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    {
                        "HealthChecks:DB2:TestContainerDegraded:Command",
                        "SELECT 1 FROM SYSIBM.SYSDUMMY1; DBMS_LOCK.SLEEP(1);"
                    },
                    { "HealthChecks:DB2:TestContainerDegraded:Timeout", "0" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDb2Devart_UseConfiguration_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDb2Devart("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:DB2:TestContainerUnhealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:DB2:TestContainerUnhealthy:Command", "invalid" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}