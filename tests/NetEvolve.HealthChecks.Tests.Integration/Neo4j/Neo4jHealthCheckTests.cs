namespace NetEvolve.HealthChecks.Tests.Integration.Neo4j;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Neo4j.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Neo4j;

[ClassDataSource<Neo4jDatabase>(Shared = InstanceSharedType.Neo4j)]
[TestGroup(nameof(Neo4j))]
public class Neo4jHealthCheckTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    private readonly Neo4jDatabase _database;
#pragma warning disable TUnit0023 // Member should be disposed within a clean up method
    private IDriver _driver = default!;
#pragma warning restore TUnit0023 // Member should be disposed within a clean up method
    private bool _disposed;

    public Neo4jHealthCheckTests(Neo4jDatabase database) => _database = database;

    public Task InitializeAsync()
    {
        _driver = GraphDatabase.Driver(new Uri(_database.ConnectionString));

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
                _driver.Dispose();
            }

            _disposed = true;
        }
    }

    [Test]
    public async Task AddNeo4j_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNeo4j("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_driver)
        );

    [Test]
    public async Task AddNeo4j_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddNeo4j(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "neo4j-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("neo4j-test", (_, _) => _driver)
        );

    [Test]
    public async Task AddNeo4j_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks => healthChecks.AddNeo4j("TestContainerHealthy").AddNeo4j("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_driver)
                )
        );

    [Test]
    public async Task AddNeo4j_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddNeo4j(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (driver, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            await using var session = driver.AsyncSession();

                            return await session.RunAsync("RETURN 1", cancellationToken).ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_driver)
        );

    [Test]
    public async Task AddNeo4j_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddNeo4j(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (driver, cancellationToken) =>
                        {
                            await using var session = driver.AsyncSession();

                            return await session.RunAsync("INVALID QUERY", cancellationToken).ConfigureAwait(false);
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_driver)
        );

    [Test]
    public async Task AddNeo4j_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNeo4j("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Neo4j:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_driver)
        );

    [Test]
    public async Task AddNeo4j_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNeo4j("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Neo4j:TestContainerKeyedHealthy:KeyedService", "neo4j-test-config" },
                    { "HealthChecks:Neo4j:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("neo4j-test-config", (_, _) => _driver)
        );

    [Test]
    public async Task AddNeo4j_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNeo4j("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Neo4j:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_driver)
        );

    [Test]
    public async Task AddNeo4j_UseConfiguration_ConnectionStringEmpty_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNeo4j("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Neo4j:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_driver)
        );

    [Test]
    public async Task AddNeo4j_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddNeo4j("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Neo4j:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_driver)
        );
}
