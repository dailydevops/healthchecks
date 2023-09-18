namespace NetEvolve.HealthChecks.Oracle.Tests.Integration;

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
public class OracleCheckTests : HealthCheckTestBase, IClassFixture<OracleDatabase>
{
    private readonly OracleDatabase _database;

    public OracleCheckTests(OracleDatabase database) => _database = database;

    [Fact]
    public async Task AddOracle_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddOracle(
                "TestContainerHealthy",
                options =>
                {
                    options.ConnectionString = _database.GetConnectionString();
                }
            );
        });

    [Fact]
    public async Task AddOracle_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                {
                    _ = healthChecks
                        .AddOracle("TestContainerHealthy")
                        .AddOracle("TestContainerHealthy");
                });
            }
        );

    [Fact]
    public async Task AddOracle_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddOracle(
                "TestContainerDegraded",
                options =>
                {
                    options.ConnectionString = _database.GetConnectionString();
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddOracle_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddOracle(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.GetConnectionString();
                    options.Command = "SELECT 1 = `1`;";
                }
            );
        });

    [Fact]
    public async Task AddOracle_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracle("TestContainerHealthy");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:OracleTestContainerHealthy:ConnectionString",
                        _database.GetConnectionString()
                    }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddOracle_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracle("TestContainerDegraded");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:OracleTestContainerDegraded:ConnectionString",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:OracleTestContainerDegraded:Timeout", "0" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddOracle_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracle("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:OracleTestNoValues:ConnectionString", "" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddOracle_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOracle("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:OracleTestNoValues:ConnectionString",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:OracleTestNoValues:Timeout", "-2" }
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
