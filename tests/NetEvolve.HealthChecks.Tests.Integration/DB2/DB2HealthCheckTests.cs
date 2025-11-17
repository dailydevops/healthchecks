namespace NetEvolve.HealthChecks.Tests.Integration.DB2;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.DB2;
using TUnit.Core.Enums;

[ClassDataSource<DB2Database>(Shared = SharedType.PerClass)]
[TestGroup(nameof(DB2))]
[TestGroup("Z02TestGroup")]
[RunOn(OS.Windows)]
public class DB2HealthCheckTests : HealthCheckTestBase
{
    private readonly DB2Database _database;

    public DB2HealthCheckTests(DB2Database database) => _database = database;

    [Test]
    public async Task AddDB2_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDB2(
                    "TestContainerHealthy",
                    options => options.ConnectionString = _database.ConnectionString
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddDB2_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDB2(
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
    public async Task AddDB2_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDB2(
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
    public async Task AddDB2_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2("TestContainerHealthy"),
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
    public async Task AddDB2_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2("TestContainerDegraded"),
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
                    { "HealthChecks:DB2:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddDB2_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2("TestNoValues"),
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
    public async Task AddDB2_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddDB2("TestNoValues"),
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
