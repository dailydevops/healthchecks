namespace NetEvolve.HealthChecks.Tests.Integration.JanusGraph;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::JanusGraph.Net.IO.GraphSON;
using Gremlin.Net.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.JanusGraph;

[ClassDataSource<JanusGraphDatabase>(Shared = SharedType.PerClass)]
[TestGroup(nameof(JanusGraph))]
[TestGroup("Z01TestGroup")]
public class JanusGraphHealthCheckTests : HealthCheckTestBase
{
    private readonly JanusGraphDatabase _database;
    private readonly JanusGraphGraphSONMessageSerializer _serializer;

    public JanusGraphHealthCheckTests(JanusGraphDatabase database)
    {
        _database = database;
        _serializer = new JanusGraphGraphSONMessageSerializer();
    }

    [Test]
    [SkipOnFailure]
    public async Task AddJanusGraph_UseOptions_Healthy()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestContainerHealthy", options => options.Timeout = 25000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton<IGremlinClient>(client)
        );
    }

    [Test]
    [SkipOnFailure]
    public async Task AddJanusGraph_UseOptionsWithKeyedService_Healthy()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddJanusGraph(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "janusgraph-test";
                        options.Timeout = 25000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton<IGremlinClient>("janusgraph-test", (_, _) => client)
        );
    }

    [Test]
    [SkipOnFailure]
    public async Task AddJanusGraph_UseOptionsDoubleRegistered_Healthy()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddJanusGraph("TestContainerHealthy").AddJanusGraph("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton<IGremlinClient>(client)
                )
        );
    }

    [Test]
    public async Task AddJanusGraph_UseOptions_Degraded()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddJanusGraph(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);
                            return true;
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton<IGremlinClient>(client)
        );
    }

    [Test]
    public async Task AddJanusGraph_UseOptions_Unhealthy()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddJanusGraph(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = (client, _) => Task.FromResult(false);
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton<IGremlinClient>(client)
        );
    }

    [Test]
    [SkipOnFailure]
    public async Task AddJanusGraph_UseConfiguration_Healthy()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:JanusGraph:TestContainerHealthy:Timeout", "25000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton<IGremlinClient>(client)
        );
    }

    [Test]
    [SkipOnFailure]
    public async Task AddJanusGraph_UseConfigurationWithKeyedService_Healthy()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:JanusGraph:TestContainerKeyedHealthy:KeyedService", "janusgraph-test-config" },
                    { "HealthChecks:JanusGraph:TestContainerKeyedHealthy:Timeout", "25000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddKeyedSingleton<IGremlinClient>("janusgraph-test-config", (_, _) => client)
        );
    }

    [Test]
    public async Task AddJanusGraph_UseConfiguration_Degraded()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:JanusGraph:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton<IGremlinClient>(client)
        );
    }

    [Test]
    public async Task AddJanusGraph_UseConfiguration_TimeoutMinusTwo_ShouldThrowException()
    {
        using var client = new GremlinClient(_database.Server, _serializer);
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:JanusGraph:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton<IGremlinClient>(client)
        );
    }
}
