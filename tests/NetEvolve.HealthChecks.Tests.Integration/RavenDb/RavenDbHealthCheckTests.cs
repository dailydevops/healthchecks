namespace NetEvolve.HealthChecks.Tests.Integration.RavenDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RavenDb;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.ServerWide.Operations;

[ClassDataSource<RavenDbDatabase>(Shared = InstanceSharedType.RavenDb)]
[TestGroup(nameof(RavenDb))]
public class RavenDbHealthCheckTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    private readonly RavenDbDatabase _database;
    private IDocumentStore _store = default!;
    private bool _disposed;

    public RavenDbHealthCheckTests(RavenDbDatabase database) => _database = database;

    public Task InitializeAsync()
    {
        var store = new DocumentStore { Urls = [_database.ConnectionString] };

        _store = store.Initialize();
        store.Dispose();

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
                _store.Dispose();
            }

            _disposed = true;
        }
    }

    [Test]
    public async Task AddRavenDb_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRavenDb("TestContainerHealthy", options => options.Timeout = 2000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_store)
        );

    [Test]
    public async Task AddRavenDb_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddRavenDb(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "mongodb-test";
                        options.Timeout = 2000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("mongodb-test", (_, _) => _store)
        );

    [Test]
    public async Task AddRavenDb_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks => healthChecks.AddRavenDb("TestContainerHealthy").AddRavenDb("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_store)
                )
        );

    [Test]
    public async Task AddRavenDb_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddRavenDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (store, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            var buildNumber = await store
                                .Maintenance.Server.SendAsync(new GetBuildNumberOperation(), cancellationToken)
                                .ConfigureAwait(false);

                            return buildNumber is not null;
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_store)
        );

    [Test]
    public async Task AddRavenDb_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRavenDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (store, cancellationToken) =>
                        {
                            var stats = await store
                                .Maintenance.ForDatabase("nonexistent-db")
                                .SendAsync(new GetStatisticsOperation(), cancellationToken);

                            return stats is not null;
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_store)
        );

    [Test]
    public async Task AddRavenDb_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRavenDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RavenDb:TestContainerHealthy:Timeout", "2000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_store)
        );

    [Test]
    public async Task AddRavenDb_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRavenDb("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RavenDb:TestContainerKeyedHealthy:KeyedService", "mongodb-test-config" },
                    { "HealthChecks:RavenDb:TestContainerKeyedHealthy:Timeout", "2000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("mongodb-test-config", (_, _) => _store)
        );

    [Test]
    public async Task AddRavenDb_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRavenDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RavenDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_store)
        );

    [Test]
    public async Task AddRavenDb_UseConfiguration_ConnectionStringEmpty_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRavenDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RavenDb:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_store)
        );

    [Test]
    public async Task AddRavenDb_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRavenDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RavenDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_store)
        );
}
