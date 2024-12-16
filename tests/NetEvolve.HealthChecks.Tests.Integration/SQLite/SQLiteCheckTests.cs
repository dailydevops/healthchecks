namespace NetEvolve.HealthChecks.Tests.Integration.SQLite;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.HealthChecks.SQLite;
using Xunit;

public class SQLiteCheckTests : HealthCheckTestBase, IClassFixture<SQLiteDatabase>
{
    private readonly SQLiteDatabase _database;

    public SQLiteCheckTests(SQLiteDatabase database) => _database = database;

    [Fact]
    public async Task AddSQLite_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSQLite(
                "TestContainerHealthy",
                options => options.ConnectionString = _database.ConnectionString
            );
        });

    [Fact]
    public async Task AddSQLite_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                {
                    _ = healthChecks
                        .AddSQLite("TestContainerHealthy")
                        .AddSQLite("TestContainerHealthy");
                });
            }
        );

    [Fact]
    public async Task AddSQLite_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSQLite(
                "TestContainerDegraded",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddSQLite_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSQLite(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "SELECT 1 = `1`";
                }
            );
        });

    [Fact]
    public async Task AddSQLite_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSQLite("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SQLite:TestContainerHealthy:ConnectionString",
                        _database.ConnectionString
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSQLite_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSQLite("TestContainerDegraded"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SQLite:TestContainerDegraded:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:SQLite:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSQLite_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSQLite("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SQLite:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSQLite_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSQLite("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SQLite:TestNoValues:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:SQLite:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
