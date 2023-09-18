namespace NetEvolve.HealthChecks.MySql.Connector.Tests.Integration;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
[SetCulture]
public class MySqlCheckTests : HealthCheckTestBase, IClassFixture<MySqlDatabase>
{
    private readonly MySqlDatabase _database;

    public MySqlCheckTests(MySqlDatabase database) => _database = database;

    [Fact]
    public async Task AddMySql_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddMySql(
                "TestContainerHealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddMySql_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                {
                    _ = healthChecks
                        .AddMySql("TestContainerHealthy")
                        .AddMySql("TestContainerHealthy");
                });
            }
        );

    [Fact]
    public async Task AddMySql_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddMySql(
                "TestContainerDegraded",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "SELECT 1; DO SLEEP(1);";
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddMySql_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddMySql(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command =
                        "SIGNAL SQLSTATE '45001' SET MESSAGE_TEXT = 'This is a test.';";
                }
            );
        });

    [Fact]
    public async Task AddMySql_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySql("TestContainerHealthy");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:MySqlTestContainerHealthy:ConnectionString",
                        _database.ConnectionString
                    }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddMySql_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySql("TestContainerDegraded");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:MySqlTestContainerDegraded:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:MySqlTestContainerDegraded:Timeout", "0" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddMySql_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySql("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:MySqlTestNoValues:ConnectionString", "" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddMySql_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMySql("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:MySqlTestNoValues:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:MySqlTestNoValues:Timeout", "-2" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
