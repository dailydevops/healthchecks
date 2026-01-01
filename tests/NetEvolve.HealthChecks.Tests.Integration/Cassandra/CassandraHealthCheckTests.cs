namespace NetEvolve.HealthChecks.Tests.Integration.Cassandra;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Cassandra;

[ClassDataSource<CassandraDatabase>(Shared = SharedType.PerClass)]
[TestGroup(nameof(Cassandra))]
[TestGroup("Z04TestGroup")]
public class CassandraHealthCheckTests : HealthCheckTestBase
{
    private readonly CassandraDatabase _database;

    public CassandraHealthCheckTests(CassandraDatabase database) => _database = database;

    [Test]
    public async Task AddCassandra_UseOptions_Healthy()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddCassandra("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton<ICluster>(cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseOptionsWithKeyedService_Healthy()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

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
            serviceBuilder: services => services.AddKeyedSingleton<ICluster>("cassandra-test", (_, _) => cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseOptionsDoubleRegistered_Healthy()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddCassandra("TestContainerHealthy").AddCassandra("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton<ICluster>(cluster)
                )
        );
    }

    [Test]
    public async Task AddCassandra_UseOptions_Degraded()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

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
                                .ExecuteAsync(new SimpleStatement("SELECT release_version FROM system.local"))
                                .ConfigureAwait(false);

                            return result?.Any() == true;
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton<ICluster>(cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseOptions_Unhealthy()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCassandra(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (cluster, _) =>
                        {
                            using var session = await cluster.ConnectAsync().ConfigureAwait(false);
                            var result = await session
                                .ExecuteAsync(new SimpleStatement("SELECT invalid FROM system.local"))
                                .ConfigureAwait(false);

                            return result?.Any() == true;
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton<ICluster>(cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseConfiguration_Healthy()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

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
            serviceBuilder: services => services.AddSingleton<ICluster>(cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseConfigurationWithKeyedService_Healthy()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

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
            serviceBuilder: services => services.AddKeyedSingleton<ICluster>("cassandra-test-config", (_, _) => cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseConfiguration_Degraded()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

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
            serviceBuilder: services => services.AddSingleton<ICluster>(cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseConfiguration_TimeoutMinusTwo_ShouldThrowException()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

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
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }

    [Test]
    public async Task AddCassandra_UseOptions_CommandReturnsFalse_UnhealthyWithMessage()
    {
        using var cluster = Cluster.Builder().WithConnectionString(_database.ConnectionString).Build();

        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCassandra(
                    "TestContainerInvalidResult",
                    options => options.CommandAsync = (_, _) => Task.FromResult(false)
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton<ICluster>(cluster)
        );
    }
}
