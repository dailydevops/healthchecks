namespace NetEvolve.HealthChecks.Tests.Integration.Qdrant;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Qdrant.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Qdrant;

[TestGroup(nameof(Qdrant))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<QdrantDatabase>(Shared = InstanceSharedType.Qdrant)]
public sealed class QdrantHealthCheckTests : HealthCheckTestBase
{
    private readonly QdrantDatabase _database;

    public QdrantHealthCheckTests(QdrantDatabase database) => _database = database;

    [Test]
    public async Task AddQdrant_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQdrant("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddQdrant_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQdrant("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddQdrant_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQdrant(
                    "TestContainerHealthyKeyed",
                    options =>
                    {
                        options.KeyedService = "qdrant-test";
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                _ = services.AddKeyedSingleton(
                    "qdrant-test",
                    (_, _) => new QdrantClient(_database.GrpcConnectionString)
                );
            }
        );

    [Test]
    public async Task AddQdrant_UseGrpcConnection_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQdrant("TestContainerGrpcHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddQdrant_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQdrant("TestContainerConfigHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Qdrant:TestContainerConfigHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddQdrant_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQdrant("TestContainerConfigDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Qdrant:TestContainerConfigDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddQdrant_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQdrant("TestContainerConfigKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Qdrant:TestContainerConfigKeyedHealthy:Timeout", "10000" },
                    { "HealthChecks:Qdrant:TestContainerConfigKeyedHealthy:KeyedService", "qdrant-keyed-test" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                _ = services.AddKeyedSingleton(
                    "qdrant-keyed-test",
                    (_, _) => new QdrantClient(_database.GrpcConnectionString)
                );
            }
        );
}
