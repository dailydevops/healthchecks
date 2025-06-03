namespace NetEvolve.HealthChecks.Tests.Integration.Redis;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Redis;
using StackExchange.Redis;

[TestGroup(nameof(Redis))]
[ClassDataSource<RedisContainer>(Shared = SharedType.PerTestSession)]
public class RedisHealthCheckTests : HealthCheckTestBase
{
    private readonly RedisContainer _database;

    public RedisHealthCheckTests(RedisContainer database) => _database = database;

    [Test]
    public async Task AddRedis_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedis(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Mode = ConnectionHandleMode.Create;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddRedis_UseOptionsServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedis(
                    "TestContainerHealthy",
                    options => options.ConnectionString = _database.GetConnectionString()
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton<IConnectionMultiplexer>(services =>
                {
                    var options = services.GetService<IOptionsSnapshot<RedisOptions>>();

                    return ConnectionMultiplexer.Connect(options!.Get("TestContainerHealthy").ConnectionString);
                });
            }
        );

    [Test]
    public async Task AddRedis_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRedis(
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

    [Test]
    public async Task AddRedis_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedis("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:RedisDatabase:TestContainerHealthy:ConnectionString",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:RedisDatabase:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton<IConnectionMultiplexer>(services =>
                {
                    var options = services.GetService<IOptionsSnapshot<RedisOptions>>();

                    return ConnectionMultiplexer.Connect(options!.Get("TestContainerHealthy").ConnectionString);
                });
            }
        );

    [Test]
    public async Task AddRedis_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedis("TestContainerDegraded"),
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

    [Test]
    public async Task AddRedis_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedis("TestNoValues"),
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

    [Test]
    public async Task AddRedis_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRedis("TestNoValues"),
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
