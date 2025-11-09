namespace NetEvolve.HealthChecks.Tests.Integration.EventStoreDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.EventStoreDb;

[ClassDataSource<EventStoreDbDatabase>(Shared = InstanceSharedType.EventStoreDb)]
[TestGroup(nameof(EventStoreDb))]
public class EventStoreDbHealthCheckTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    private readonly EventStoreDbDatabase _database;
    private EventStoreClient _client = default!;
    private bool _disposed;

    public EventStoreDbHealthCheckTests(EventStoreDbDatabase database) => _database = database;

    public Task InitializeAsync()
    {
        var settings = EventStoreClientSettings.Create(_database.ConnectionString);
        _client = new EventStoreClient(settings);

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
    public async Task AddEventStoreDb_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddEventStoreDb("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddEventStoreDb_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddEventStoreDb(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "eventstoredb-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("eventstoredb-test", (_, _) => _client)
        );

    [Test]
    public async Task AddEventStoreDb_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddEventStoreDb("TestContainerHealthy").AddEventStoreDb("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Test]
    public async Task AddEventStoreDb_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddEventStoreDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            var readStream = client.ReadStreamAsync(
                                Direction.Backwards,
                                "$all",
                                StreamPosition.End,
                                maxCount: 1,
                                resolveLinkTos: false,
                                cancellationToken: cancellationToken
                            );

                            await foreach (var _ in readStream.ConfigureAwait(false))
                            {
                                return true;
                            }

                            return true;
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddEventStoreDb_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddEventStoreDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.CompletedTask;
                            throw new InvalidOperationException("Test exception");
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddEventStoreDb_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddEventStoreDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:EventStoreDb:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddEventStoreDb_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddEventStoreDb("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:EventStoreDb:TestContainerKeyedHealthy:KeyedService", "eventstoredb-test-config" },
                    { "HealthChecks:EventStoreDb:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("eventstoredb-test-config", (_, _) => _client)
        );

    [Test]
    public async Task AddEventStoreDb_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddEventStoreDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:EventStoreDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddEventStoreDb_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddEventStoreDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:EventStoreDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
