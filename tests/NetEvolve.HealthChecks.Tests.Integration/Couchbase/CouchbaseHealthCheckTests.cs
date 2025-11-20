namespace NetEvolve.HealthChecks.Tests.Integration.Couchbase;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Couchbase;
using Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Couchbase;
using Testcontainers.Couchbase;

[ClassDataSource<CouchbaseDatabase>(Shared = SharedType.PerClass)]
[TestGroup(nameof(Couchbase))]
[TestGroup("Z01TestGroup")]
[Retry(25)]
public class CouchbaseHealthCheckTests : HealthCheckTestBase
{
    private readonly CouchbaseDatabase _database;

    public CouchbaseHealthCheckTests(CouchbaseDatabase database) => _database = database;

    private async Task<ICluster> CreateCluster()
    {
        var options = new ClusterOptions
        {
            ConnectionString = _database.ConnectionString,
            UserName = CouchbaseBuilder.DefaultUsername,
            Password = CouchbaseBuilder.DefaultPassword,
        };

        return await Cluster.ConnectAsync(options).ConfigureAwait(false);
    }

    [Test]
    public async Task AddCouchbase_UseOptions_Healthy()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchbase("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseOptionsWithKeyedService_Healthy()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCouchbase(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "couchbase-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("couchbase-test", (_, _) => cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseOptionsDoubleRegistered_Healthy()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddCouchbase("TestContainerHealthy").AddCouchbase("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(cluster)
                )
        );
    }

    [Test]
    public async Task AddCouchbase_UseOptions_Degraded()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCouchbase(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (cluster, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);
                            _ = await cluster.PingAsync().ConfigureAwait(false);
                            return true;
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseOptions_Unhealthy()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCouchbase(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (cluster, cancellationToken) =>
                        {
                            await Task.Delay(0, cancellationToken);
                            throw new InvalidOperationException("Simulated failure");
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseConfiguration_Healthy()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchbase("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Couchbase:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseConfigurationWithKeyedService_Healthy()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchbase("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Couchbase:TestContainerKeyedHealthy:KeyedService", "couchbase-test-config" },
                    { "HealthChecks:Couchbase:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("couchbase-test-config", (_, _) => cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseConfiguration_Degraded()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchbase("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Couchbase:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseConfiguration_ConnectionStringEmpty_ShouldThrowException()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchbase("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Couchbase:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }

    [Test]
    public async Task AddCouchbase_UseConfiguration_TimeoutMinusTwo_ShouldThrowException()
    {
        var cluster = await CreateCluster().ConfigureAwait(false);
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchbase("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Couchbase:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(cluster)
        );
    }
}
