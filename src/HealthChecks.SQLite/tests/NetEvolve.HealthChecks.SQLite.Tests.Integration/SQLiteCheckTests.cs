namespace NetEvolve.HealthChecks.SQLite.Tests.Integration;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.SQLite;
using NetEvolve.HealthChecks.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
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
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                    }
                );
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSQLite_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert
            .ThrowsAsync<ArgumentException>(
                "name",
                async () =>
                {
                    await RunAndVerify(healthChecks =>
                        {
                            _ = healthChecks
                                .AddSQLite("TestContainerHealthy")
                                .AddSQLite("TestContainerHealthy");
                        })
                        .ConfigureAwait(false);
                }
            )
            .ConfigureAwait(false);

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
            })
            .ConfigureAwait(false);

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
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSQLite_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSQLite("TestContainerHealthy");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        {
                            "HealthChecks:SQLiteTestContainerHealthy:ConnectionString",
                            _database.ConnectionString
                        }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSQLite_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSQLite("TestContainerDegraded");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        {
                            "HealthChecks:SQLiteTestContainerDegraded:ConnectionString",
                            _database.ConnectionString
                        },
                        { "HealthChecks:SQLiteTestContainerDegraded:Timeout", "0" }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSQLite_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSQLite("TestNoValues");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        { "HealthChecks:SQLiteTestNoValues:ConnectionString", "" }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSQLite_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSQLite("TestNoValues");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        {
                            "HealthChecks:SQLiteTestNoValues:ConnectionString",
                            _database.ConnectionString
                        },
                        { "HealthChecks:SQLiteTestNoValues:Timeout", "-2" }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);
}
