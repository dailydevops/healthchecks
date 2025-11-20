namespace NetEvolve.HealthChecks.Tests.Integration.Milvus;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Milvus.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Milvus;

[TestGroup(nameof(Milvus))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<MilvusDatabase>(Shared = SharedType.PerClass)]
public sealed class MilvusHealthCheckTests : HealthCheckTestBase
{
    private readonly MilvusDatabase _database;

    public MilvusHealthCheckTests(MilvusDatabase database) => _database = database;

    [Test]
    public async Task AddMilvus_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMilvus("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_ => new MilvusClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddMilvus_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMilvus("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_ => new MilvusClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddMilvus_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMilvus(
                    "TestContainerHealthyKeyed",
                    options =>
                    {
                        options.KeyedService = "milvus-test";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                _ = services.AddKeyedSingleton(
                    "milvus-test",
                    (_, _) => new MilvusClient(_database.GrpcConnectionString)
                );
            }
        );

    [Test]
    public async Task AddMilvus_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMilvus("TestContainerConfigHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Milvus:TestContainerConfigHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_ => new MilvusClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddMilvus_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMilvus("TestContainerConfigDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Milvus:TestContainerConfigDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_ => new MilvusClient(_database.GrpcConnectionString))
        );

    [Test]
    public async Task AddMilvus_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMilvus("TestContainerConfigKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Milvus:TestContainerConfigKeyedHealthy:Timeout", "10000" },
                    { "HealthChecks:Milvus:TestContainerConfigKeyedHealthy:KeyedService", "milvus-keyed-test" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                _ = services.AddKeyedSingleton(
                    "milvus-keyed-test",
                    (_, _) => new MilvusClient(_database.GrpcConnectionString)
                );
            }
        );
}
