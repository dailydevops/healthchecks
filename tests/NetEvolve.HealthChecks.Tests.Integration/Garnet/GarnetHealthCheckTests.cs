namespace NetEvolve.HealthChecks.Tests.Integration.Garnet;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Garnet;
using StackExchange.Redis;

[TestGroup(nameof(Garnet))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<GarnetContainer>(Shared = SharedType.PerClass)]
public class GarnetHealthCheckTests : HealthCheckTestBase
{
    private readonly GarnetContainer _database;

    public GarnetHealthCheckTests(GarnetContainer database) => _database = database;

    [Test]
    public async Task AddGarnet_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddGarnet(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Mode = ConnectionHandleMode.Create;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddGarnet_UseOptionsServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddGarnet(
                    "TestContainerHealthy",
                    options => options.ConnectionString = _database.GetConnectionString()
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton<IConnectionMultiplexer>(services =>
                {
                    var options = services.GetService<IOptionsSnapshot<GarnetOptions>>();

                    return ConnectionMultiplexer.Connect(options!.Get("TestContainerHealthy").ConnectionString);
                });
            }
        );

    [Test]
    public async Task AddGarnet_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddGarnet(
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
    public async Task AddGarnet_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:GarnetDatabase:TestContainerHealthy:ConnectionString",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:GarnetDatabase:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton<IConnectionMultiplexer>(services =>
                {
                    var options = services.GetService<IOptionsSnapshot<GarnetOptions>>();

                    return ConnectionMultiplexer.Connect(options!.Get("TestContainerHealthy").ConnectionString);
                });
            }
        );

    [Test]
    public async Task AddGarnet_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:GarnetDatabase:TestContainerDegraded:ConnectionString",
                        _database.GetConnectionString()
                    },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddGarnet_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:GarnetDatabase:TestNoValues:ConnectionString", "" },
                    { "HealthChecks:GarnetDatabase:TestNoValues:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddGarnet_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:GarnetDatabase:TestNoValues:ConnectionString", _database.GetConnectionString() },
                    { "HealthChecks:GarnetDatabase:TestNoValues:Timeout", "-2" },
                    { "HealthChecks:GarnetDatabase:TestNoValues:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
