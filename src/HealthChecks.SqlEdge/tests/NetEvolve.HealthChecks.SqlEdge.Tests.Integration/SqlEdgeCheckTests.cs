namespace NetEvolve.HealthChecks.SqlEdge.Tests.Integration;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Tests;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
[SetCulture("en-US")]
public class SqlEdgeCheckTests : HealthCheckTestBase, IClassFixture<SqlEdgeDatabase>
{
    private readonly SqlEdgeDatabase _database;

    public SqlEdgeCheckTests(SqlEdgeDatabase database) => _database = database;

    [Fact]
    public async Task AddSqlEdge_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSqlEdge(
                "TestContainerHealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                }
            );
        });

    [Fact]
    public async Task AddSqlEdge_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                {
                    _ = healthChecks
                        .AddSqlEdge("TestContainerHealthy")
                        .AddSqlEdge("TestContainerHealthy");
                });
            }
        );

    [Fact]
    public async Task AddSqlEdge_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSqlEdge(
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
    public async Task AddSqlEdge_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSqlEdge(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "RAISERROR('This is a test.',16,1)";
                }
            );
        });

    [Fact]
    public async Task AddSqlEdge_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlEdge("TestContainerHealthy");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SqlEdge:TestContainerHealthy:ConnectionString",
                        _database.ConnectionString
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSqlEdge_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlEdge("TestContainerDegraded");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SqlEdge:TestContainerDegraded:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:SqlEdge:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSqlEdge_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlEdge("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlEdge:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSqlEdge_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSqlEdge("TestNoValues");
            },
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SqlEdge:TestNoValues:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:SqlEdge:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
