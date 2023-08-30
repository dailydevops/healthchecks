﻿namespace NetEvolve.HealthChecks.SqlServer.Legacy.Tests.Integration;

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
public class SqlServerLegacyCheckTests : HealthCheckTestBase, IClassFixture<SqlServerLegacyDatabase>
{
    private readonly SqlServerLegacyDatabase _database;

    public SqlServerLegacyCheckTests(SqlServerLegacyDatabase database) => _database = database;

    [Fact]
    public async Task AddSqlServerLegacy_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddSqlServerLegacy(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                    }
                );
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServerLegacy_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert
            .ThrowsAsync<ArgumentException>(
                "name",
                async () =>
                {
                    await RunAndVerify(healthChecks =>
                        {
                            _ = healthChecks
                                .AddSqlServerLegacy("TestContainerHealthy")
                                .AddSqlServerLegacy("TestContainerHealthy");
                        })
                        .ConfigureAwait(false);
                }
            )
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServerLegacy_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddSqlServerLegacy(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 1;
                    }
                );
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServerLegacy_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddSqlServerLegacy(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Command = "RAISERROR('This is a test.',16,1)";
                    }
                );
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServerLegacy_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSqlServerLegacy("TestContainerHealthy");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        {
                            "HealthChecks:SqlServerTestContainerHealthy:ConnectionString",
                            _database.GetConnectionString()
                        }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServerLegacy_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSqlServerLegacy("TestContainerDegraded");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        {
                            "HealthChecks:SqlServerTestContainerDegraded:ConnectionString",
                            _database.GetConnectionString()
                        },
                        { "HealthChecks:SqlServerTestContainerDegraded:Timeout", "0" }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);
}
