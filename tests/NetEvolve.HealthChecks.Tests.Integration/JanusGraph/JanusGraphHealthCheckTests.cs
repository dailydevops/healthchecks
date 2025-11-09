namespace NetEvolve.HealthChecks.Tests.Integration.JanusGraph;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.JanusGraph;

[ClassDataSource<JanusGraphDatabase>(Shared = InstanceSharedType.JanusGraph)]
[TestGroup(nameof(JanusGraph))]
public class JanusGraphHealthCheckTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    private readonly JanusGraphDatabase _database;
#pragma warning disable TUnit0023 // Member should be disposed within a clean up method
    private GremlinClient _client = default!;
#pragma warning restore TUnit0023 // Member should be disposed within a clean up method
    private bool _disposed;

    public JanusGraphHealthCheckTests(JanusGraphDatabase database) => _database = database;

    public Task InitializeAsync()
    {
        var server = new GremlinServer(_database.HostUrl, 8182);
        _client = new GremlinClient(server);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _client.Dispose();
            }

            _disposed = true;
        }
    }

    [Test]
    public async Task AddJanusGraph_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddJanusGraph_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddJanusGraph(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "janusgraph-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("janusgraph-test", (_, _) => _client)
        );

    [Test]
    public async Task AddJanusGraph_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddJanusGraph("TestContainerHealthy").AddJanusGraph("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Test]
    public async Task AddJanusGraph_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddJanusGraph(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

#pragma warning disable CA2016 // Forward the CancellationToken parameter to methods
                            return await client.SubmitAsync<int>("g.V().limit(1).count()").ConfigureAwait(false);
#pragma warning restore CA2016 // Forward the CancellationToken parameter to methods
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddJanusGraph_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddJanusGraph(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
#pragma warning disable CA2016 // Forward the CancellationToken parameter to methods
                            return await client.SubmitAsync<int>("invalid_gremlin_query").ConfigureAwait(false);
#pragma warning restore CA2016 // Forward the CancellationToken parameter to methods
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddJanusGraph_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:JanusGraph:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddJanusGraph_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddJanusGraph("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:JanusGraph:TestContainerKeyedHealthy:KeyedService", "janusgraph-test-config" },
                    { "HealthChecks:JanusGraph:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("janusgraph-test-config", (_, _) => _client)
        );

    [Test]
    public async Task AddJanusGraph_UseConfiguration_Degraded() =>
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
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddJanusGraph_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
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
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
