namespace NetEvolve.HealthChecks.Tests.Integration.Redis;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Redis;
using StackExchange.Redis;
using Xunit;

[SetCulture("", asHiddenCategory: true)]
[TestGroup(nameof(Redis))]
public class RedisDatabaseCheckTests : HealthCheckTestBase, IClassFixture<RedisDatabase>
{
    private readonly RedisDatabase _database;

    public RedisDatabaseCheckTests(RedisDatabase database) => _database = database;

    [Fact]
    public async Task AddRedisDatabase_UseOptionsCreate_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedisDatabase(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Mode = ConnectionHandleMode.Create;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Fact]
    public async Task AddRedisDatabase_UseOptionsServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedisDatabase(
                    "TestContainerHealthy",
                    options => options.ConnectionString = _database.GetConnectionString()
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton<IConnectionMultiplexer>(services =>
                {
                    var options = services.GetService<IOptionsSnapshot<RedisDatabaseOptions>>();

                    return ConnectionMultiplexer.Connect(options!.Get("TestContainerHealthy").ConnectionString);
                });
            }
        );

    [Fact]
    public async Task AddRedisDatabase_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddRedisDatabase("TestContainerHealthy").AddRedisDatabase("TestContainerHealthy"),
                    HealthStatus.Healthy
                )
        );

    [Fact]
    public async Task AddRedisDatabase_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedisDatabase(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Timeout = 0;
                        options.Mode = ConnectionHandleMode.Create;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Fact]
    public async Task AddRedisDatabase_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedisDatabase("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:RedisDatabase:TestContainerHealthy:ConnectionString",
                        _database.GetConnectionString()
                    },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton<IConnectionMultiplexer>(services =>
                {
                    var options = services.GetService<IOptionsSnapshot<RedisDatabaseOptions>>();

                    return ConnectionMultiplexer.Connect(options!.Get("TestContainerHealthy").ConnectionString);
                });
            }
        );

    [Fact]
    public async Task AddRedisDatabase_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedisDatabase("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:RedisDatabase:TestContainerDegraded:ConnectionString",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:RedisDatabase:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:RedisDatabase:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddRedisDatabase_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedisDatabase("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RedisDatabase:TestNoValues:ConnectionString", "" },
                    { "HealthChecks:RedisDatabase:TestNoValues:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddRedisDatabase_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedisDatabase("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RedisDatabase:TestNoValues:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:RedisDatabase:TestNoValues:Timeout", "-2" },
                    { "HealthChecks:RedisDatabase:TestNoValues:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
