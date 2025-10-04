namespace NetEvolve.HealthChecks.Tests.Integration.Azure.CosmosDB;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[ClassDataSource<CosmosDbDatabase>(Shared = InstanceSharedType.CosmosDb)]
[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public class CosmosDbHealthCheckIntegrationTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    private readonly CosmosDbDatabase _database;
    private CosmosClient _client = default!;
    private bool _disposed;

    public CosmosDbHealthCheckIntegrationTests(CosmosDbDatabase database) => _database = database;

    public async Task InitializeAsync()
    {
        _client = new CosmosClient(_database.ConnectionString);

        // Create a test database and container
        var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync("TestDatabase");
        var database = databaseResponse.Database;
        _ = await database.CreateContainerIfNotExistsAsync("TestContainer", "/id");
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
                _client?.Dispose();
            }

            _disposed = true;
        }
    }

    [Test]
    public async Task AddCosmosDb_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCosmosDb("TestContainerHealthy", options => options.Timeout = 5000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCosmosDb(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "cosmosdb-test";
                        options.Timeout = 5000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("cosmosdb-test", (_, _) => _client)
        );

    [Test]
    public async Task AddCosmosDb_UseOptionsDoubleRegistered_ThrowsException() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddCosmosDb("TestContainerHealthy").AddCosmosDb("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Test]
    public async Task AddCosmosDb_UseOptions_WithDatabaseName_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCosmosDb(
                    "TestContainerDatabaseHealthy",
                    options =>
                    {
                        options.DatabaseName = "TestDatabase";
                        options.Timeout = 5000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseOptions_WithDatabaseAndContainerName_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCosmosDb(
                    "TestContainerFullHealthy",
                    options =>
                    {
                        options.DatabaseName = "TestDatabase";
                        options.ContainerName = "TestContainer";
                        options.Timeout = 5000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCosmosDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseOptions_WithNonExistentDatabase_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.DatabaseName = "NonExistentDatabase";
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseOptions_WithNonExistentContainer_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCosmosDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.DatabaseName = "TestDatabase";
                        options.ContainerName = "NonExistentContainer";
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCosmosDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CosmosDb:TestContainerHealthy:Timeout", "5000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCosmosDb("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:CosmosDb:TestContainerKeyedHealthy:KeyedService", "cosmosdb-test-config" },
                    { "HealthChecks:CosmosDb:TestContainerKeyedHealthy:Timeout", "5000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("cosmosdb-test-config", (_, _) => _client)
        );

    [Test]
    public async Task AddCosmosDb_UseConfiguration_WithDatabaseName_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCosmosDb("TestContainerDatabaseHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CosmosDb:TestContainerDatabaseHealthy:DatabaseName", "TestDatabase" },
                    { "HealthChecks:CosmosDb:TestContainerDatabaseHealthy:Timeout", "5000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCosmosDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CosmosDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddCosmosDb_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCosmosDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CosmosDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
