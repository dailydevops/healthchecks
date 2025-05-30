namespace NetEvolve.HealthChecks.Tests.Integration.SqlServerLegacy;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.SqlServer.Legacy;
using NetEvolve.HealthChecks.Tests.Integration.SqlServer;

[TestGroup(nameof(SqlServerLegacy))]
[ClassDataSource<SqlServerDatabase>(Shared = SharedType.PerTestSession)]
public class SqlServerLegacyHealthCheckTests : HealthCheckTestBase
{
    private readonly SqlServerDatabase _database;

    public SqlServerLegacyHealthCheckTests(SqlServerDatabase database) => _database = database;

    [Test]
    public async Task AddSqlServerLegacy_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServerLegacy(
                    "TestContainerHealthy",
                    options => options.ConnectionString = _database.ConnectionString
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddSqlServerLegacy_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServerLegacy(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddSqlServerLegacy_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlServerLegacy(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "RAISERROR('This is a test.',16,1)";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddSqlServerLegacy_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerLegacy("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSqlServerLegacy_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerLegacy("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSqlServerLegacy_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerLegacy("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSqlServerLegacy_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSqlServerLegacy("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:SqlServer:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
