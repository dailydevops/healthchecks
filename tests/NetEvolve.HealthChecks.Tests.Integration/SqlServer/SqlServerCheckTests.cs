namespace NetEvolve.HealthChecks.Tests.Integration.SqlServer;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.SqlServer;
using NetEvolve.HealthChecks.Tests;
using Xunit;

[SetCulture("en-US")]
public class SqlServerCheckTests : HealthCheckTestBase, IClassFixture<SqlServerDatabase>
{
    private readonly SqlServerDatabase _database;

    public SqlServerCheckTests(SqlServerDatabase database) => _database = database;

    [Fact]
    public async Task AddSqlServer_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSqlServer(
                "TestContainerHealthy",
                options => options.ConnectionString = _database.ConnectionString
            );
        });

    [Fact]
    public async Task AddSqlServer_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                {
                    _ = healthChecks
                        .AddSqlServer("TestContainerHealthy")
                        .AddSqlServer("TestContainerHealthy");
                });
            }
        );

    [Fact]
    public async Task AddSqlServer_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSqlServer(
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
    public async Task AddSqlServer_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddSqlServer(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "RAISERROR('This is a test.',16,1)";
                }
            );
        });

    [Fact]
    public async Task AddSqlServer_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSqlServer("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SqlServer:TestContainerHealthy:ConnectionString",
                        _database.ConnectionString
                    },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSqlServer_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSqlServer("TestContainerDegraded"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SqlServer:TestContainerDegraded:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:SqlServer:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSqlServer_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSqlServer("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:SqlServer:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddSqlServer_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddSqlServer("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "HealthChecks:SqlServer:TestNoValues:ConnectionString",
                        _database.ConnectionString
                    },
                    { "HealthChecks:SqlServer:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
