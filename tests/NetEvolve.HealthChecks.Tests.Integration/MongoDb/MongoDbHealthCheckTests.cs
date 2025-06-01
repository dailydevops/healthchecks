namespace NetEvolve.HealthChecks.Tests.Integration.MongoDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.MongoDb;
using Xunit;

[TestGroup(nameof(MongoDb))]
public class MongoDbHealthCheckTests : HealthCheckTestBase, IClassFixture<MongoDbDatabase>, IDisposable
{
    private readonly MongoDbDatabase _database;
    private readonly MongoClient _client;
    private bool _disposed;

    public MongoDbHealthCheckTests(MongoDbDatabase database)
    {
        _database = database;
        _client = new MongoClient(_database.ConnectionString);
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

    [Fact]
    public async Task AddMongoDb_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestContainerHealthy", options => options.Timeout = 1000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Fact]
    public async Task AddMongoDb_UseOptionsWithKeyedService_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddMongoDb(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "mongodb-test";
                        options.Timeout = 1000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("mongodb-test", (_, _) => _client)
        );

    [Fact]
    public async Task AddMongoDb_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks => healthChecks.AddMongoDb("TestContainerHealthy").AddMongoDb("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Fact]
    public async Task AddMongoDb_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddMongoDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (MongoClient client, CancellationToken cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            var database = client.GetDatabase("admin");
                            var command = new BsonDocument("ping", 1);

                            return await database
                                .RunCommandAsync<BsonDocument>(command, null, cancellationToken)
                                .ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Fact]
    public async Task AddMongoDb_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMongoDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (MongoClient client, CancellationToken cancellationToken) =>
                        {
                            var database = client.GetDatabase("admin");
                            var invalidCommand = new BsonDocument("invalid", 1);

                            return await database
                                .RunCommandAsync<BsonDocument>(invalidCommand, null, cancellationToken)
                                .ConfigureAwait(false);
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Fact]
    public async Task AddMongoDb_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Fact]
    public async Task AddMongoDb_UseConfigurationWithKeyedService_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:MongoDb:TestContainerKeyedHealthy:KeyedService", "mongodb-test-config" },
                    { "HealthChecks:MongoDb:TestContainerKeyedHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("mongodb-test-config", (_, _) => _client)
        );

    [Fact]
    public async Task AddMongoDb_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Fact]
    public async Task AddMongoDb_UseConfigration_ConnectionStringEmpty_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Fact]
    public async Task AddMongoDb_UseConfigration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
