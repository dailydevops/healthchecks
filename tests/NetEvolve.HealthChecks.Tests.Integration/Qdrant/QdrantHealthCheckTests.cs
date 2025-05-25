namespace NetEvolve.HealthChecks.Tests.Integration.Qdrant;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Qdrant.Client;
using global::Qdrant.Client.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Qdrant;
using Xunit;

[TestGroup(nameof(Qdrant))]
public sealed class QdrantHealthCheckTests : HealthCheckTestBase, IClassFixture<QdrantDatabase>
{
    private readonly QdrantDatabase _database;

    public QdrantHealthCheckTests(QdrantDatabase database) => _database = database;

    [Fact]
    public async Task AddQdrant_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddQdrant("TestContainerHealthy"),
            serviceBuilder: services => _ = new QdrantClient(_database.GrpcConnectionString)
        );

    [Fact]
    public async Task AddQdrant_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(
                    healthChecks =>
                        _ = healthChecks.AddQdrant("TestContainerHealthy").AddQdrant("TestContainerHealthy"),
                    serviceBuilder: services =>
                        _ = services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
                );
            }
        );

    [Fact]
    public async Task AddQdrant_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddQdrant("TestContainerDegraded", options => options.Timeout = 0),
            serviceBuilder: services => _ = services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Fact]
    public async Task AddQdrant_UseOptions_WithKeyedService_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddQdrant(
                    "TestContainerHealthyKeyed",
                    options => options.KeyedService = "qdrant-test"
                );
            },
            serviceBuilder: services =>
            {
                _ = services.AddKeyedSingleton(
                    "qdrant-test",
                    (_, _) => new QdrantClient(_database.GrpcConnectionString)
                );
            }
        );

    [Fact]
    public async Task AddQdrant_UseGrpcConnection_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddQdrant("TestContainerGrpcHealthy"),
            serviceBuilder: services => _ = services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Fact]
    public async Task AddQdrant_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddQdrant("TestContainerConfigHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Qdrant:TestContainerConfigHealthy:Timeout", "100" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Fact]
    public async Task AddQdrant_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddQdrant("TestContainerConfigDegraded"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Qdrant:TestContainerConfigDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => new QdrantClient(_database.GrpcConnectionString))
        );

    [Fact]
    public async Task AddQdrant_UseConfiguration_WithKeyedService_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddQdrant("TestContainerConfigKeyedHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Qdrant:TestContainerConfigKeyedHealthy:Timeout", "100" },
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
