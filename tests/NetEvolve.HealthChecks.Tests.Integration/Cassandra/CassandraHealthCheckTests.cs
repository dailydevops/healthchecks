namespace NetEvolve.HealthChecks.Tests.Integration.Cassandra;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CassandraDriver = global::Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;

[ClassDataSource<CassandraDatabase>]
[TestGroup(nameof(Cassandra))]
[TestGroup("Z04TestGroup")]
public class CassandraHealthCheckTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    private readonly CassandraDatabase _database;
#pragma warning disable TUnit0023 // Member should be disposed within a clean up method
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private CassandraDriver.Cluster _cluster = default!;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
#pragma warning restore TUnit0023 // Member should be disposed within a clean up method
    private bool _disposed;

    public CassandraHealthCheckTests(CassandraDatabase database) => _database = database;

    public Task InitializeAsync()
    {
        _cluster = CassandraDriver.Cluster.Builder().AddContactPoint(_database.ConnectionString).Build();

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
                _cluster?.Dispose();
            }

            _disposed = true;
        }
    }

    [Test]
    public async Task AddCassandra_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCassandra("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_cluster)
        );

    [Test]
    public async Task AddCassandra_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCassandra(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "cassandra-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("cassandra-test", (_, _) => _cluster)
        );

    [Test]
    public async Task AddCassandra_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks => healthChecks.AddCassandra("TestContainerHealthy").AddCassandra("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_cluster)
                )
        );

    [Test]
    public async Task AddCassandra_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCassandra(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (cluster, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            using var session = await cluster.ConnectAsync().ConfigureAwait(false);
                            var result = await session
                                .ExecuteAsync(new CassandraDriver.SimpleStatement("SELECT release_version FROM system.local"))
                                .ConfigureAwait(false);

                            return result is not null && result.Any();
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_cluster)
        );

    [Test]
    public async Task AddCassandra_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCassandra(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (cluster, cancellationToken) =>
                        {
                            using var session = await cluster.ConnectAsync().ConfigureAwait(false);
                            var result = await session
                                .ExecuteAsync(new CassandraDriver.SimpleStatement("SELECT invalid FROM system.local"))
                                .ConfigureAwait(false);

                            return result is not null && result.Any();
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_cluster)
        );

    [Test]
    public async Task AddCassandra_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCassandra("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Cassandra:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_cluster)
        );

    [Test]
    public async Task AddCassandra_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCassandra("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Cassandra:TestContainerKeyedHealthy:KeyedService", "cassandra-test-config" },
                    { "HealthChecks:Cassandra:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("cassandra-test-config", (_, _) => _cluster)
        );

    [Test]
    public async Task AddCassandra_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCassandra("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Cassandra:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_cluster)
        );

    [Test]
    public async Task AddCassandra_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCassandra("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Cassandra:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_cluster)
        );
}
