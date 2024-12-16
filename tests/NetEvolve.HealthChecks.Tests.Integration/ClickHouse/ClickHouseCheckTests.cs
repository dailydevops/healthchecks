namespace NetEvolve.HealthChecks.Tests.Integration.ClickHouse;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.ClickHouse;
using NetEvolve.HealthChecks.Tests;
using Xunit;

[SetCulture("en-US")]
public class ClickHouseCheckTests : HealthCheckTestBase, IClassFixture<ClickHouseDatabase>
{
    private readonly ClickHouseDatabase _database;

    public ClickHouseCheckTests(ClickHouseDatabase database) => _database = database;

    [Fact]
    public async Task AddClickHouse_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddClickHouse(
                "TestContainerHealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddClickHouse_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                {
                    _ = healthChecks
                        .AddClickHouse("TestContainerHealthy")
                        .AddClickHouse("TestContainerHealthy");
                });
            }
        );

    [Fact]
    public async Task AddClickHouse_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddClickHouse(
                "TestContainerDegraded",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "SELECT 1;";
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddClickHouse_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddClickHouse(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "Error";
                }
            );
        });

    [Fact]
    public async Task AddClickHouse_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse("TestContainerHealthy");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ClickHouse:TestContainerHealthy:ConnectionString",
                        _database.ConnectionString
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddClickHouse_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse("TestContainerDegraded");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ClickHouse:TestContainerDegraded:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:ClickHouse:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddClickHouse_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ClickHouse:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddClickHouse_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddClickHouse("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:ClickHouse:TestNoValues:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:ClickHouse:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
