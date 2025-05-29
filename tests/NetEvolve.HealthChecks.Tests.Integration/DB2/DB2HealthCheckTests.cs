namespace NetEvolve.HealthChecks.Tests.Integration.DB2;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.DB2;

[TestGroup(nameof(DB2))]
public class DB2HealthCheckTests : HealthCheckTestBase, IClassFixture<DB2Database>
{
    private readonly DB2Database _database;

    public DB2HealthCheckTests(DB2Database database) => _database = database;

    [Fact]
    public async Task AddDB2_UseOptions_ShouldReturnHealthy() =>
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

    [Fact]
    public async Task AddDB2_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(
                    healthChecks => healthChecks.AddDB2("TestContainerHealthy").AddDB2("TestContainerHealthy"),
                    HealthStatus.Healthy
                );
            }
        );

    [Fact]
    public async Task AddDB2_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddDB2(
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

    [Fact]
    public async Task AddDB2_UseOptions_ShouldReturnUnhealthy() =>
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

    [Fact]
    public async Task AddDB2_UseConfiguration_ShouldReturnHealthy() =>
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

    [Fact]
    public async Task AddDB2_UseConfiguration_ShouldReturnDegraded() =>
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

    [Fact]
    public async Task AddDB2_UseConfigration_ConnectionStringEmpty_ThrowException() =>
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

    [Fact]
    public async Task AddDB2_UseConfigration_TimeoutMinusTwo_ThrowException() =>
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
