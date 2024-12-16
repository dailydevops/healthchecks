namespace NetEvolve.HealthChecks.Tests.Integration.Npgsql;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.HealthChecks.Npgsql;
using Xunit;

public class NpgsqlCheckTests : HealthCheckTestBase, IClassFixture<NpgsqlDatabase>
{
    private readonly NpgsqlDatabase _database;

    public NpgsqlCheckTests(NpgsqlDatabase database) => _database = database;

    [Fact]
    public async Task AddPostgreSql_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddPostgreSql(
                "TestContainerHealthy",
                options => options.ConnectionString = _database.ConnectionString
            );
        });

    [Fact]
    public async Task AddPostgreSql_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                {
                    _ = healthChecks
                        .AddPostgreSql("TestContainerHealthy")
                        .AddPostgreSql("TestContainerHealthy");
                });
            }
        );

    [Fact]
    public async Task AddPostgreSql_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddPostgreSql(
                "TestContainerDegraded",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddPostgreSql_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddPostgreSql(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "SELECT 1 = `1`";
                }
            );
        });

    [Fact]
    public async Task AddPostgreSql_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddPostgreSql("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:PostgreSql:TestContainerHealthy:ConnectionString",
                        _database.ConnectionString
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddPostgreSql_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddPostgreSql("TestContainerDegraded"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:PostgreSql:TestContainerDegraded:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:PostgreSql:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddPostgreSql_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddPostgreSql("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:PostgreSql:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddPostgreSql_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddPostgreSql("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:PostgreSql:TestNoValues:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:PostgreSql:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
