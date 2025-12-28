namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArangoDBNetStandard;
using ArangoDBNetStandard.Transport.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.ArangoDb;
using NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

public abstract class ArangoDbHealthCheckBaseTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    public ContainerBase _container { get; }
#pragma warning disable TUnit0023 // Member should be disposed within a clean up method
    private ArangoDBClient _client = default!;
#pragma warning restore TUnit0023 // Member should be disposed within a clean up method
    private bool _disposed;

    protected ArangoDbHealthCheckBaseTests(ContainerBase container) => _container = container;

    public Task InitializeAsync()
    {
        var transport = HttpApiTransport.UsingBasicAuth(
            new Uri(_container.TransportAddress),
            "root",
            _container.Password
        );
        _client = new ArangoDBClient(transport);

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
    public async Task AddArangoDb_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseOptionsWithKeyedService_Healthy()
    {
        const string serviceKey = "options-test-key";

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddArangoDb(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = serviceKey;
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton(serviceKey, (_, _) => _client)
        );
    }

    [Test]
    public async Task AddArangoDb_UseOptionsWithInternalMode_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddArangoDb(
                    "TestContainerInternalHealthy",
                    options =>
                    {
                        options.Mode = ArangoDbClientCreationMode.Internal;
                        options.Timeout = 10000;
                        options.TransportAddress = _container.TransportAddress;
                        options.Username = "root";
                        options.Password = _container.Password;
                    }
                ),
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddArangoDb_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddArangoDb("TestContainerHealthy").AddArangoDb("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Test]
    public async Task AddArangoDb_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddArangoDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            return await ArangoDbHealthCheck
                                .DefaultCommandAsync(client, cancellationToken)
                                .ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddArangoDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (_, _) =>
                        {
                            await Task.CompletedTask;
                            throw new InvalidOperationException("test");
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ArangoDb:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseConfigurationWithKeyedService_Healthy()
    {
        const string serviceKey = "config-test-key";

        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:ArangoDb:TestContainerKeyedHealthy:KeyedService", serviceKey },
                    { "HealthChecks:ArangoDb:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton(serviceKey, (_, _) => _client)
        );
    }

    [Test]
    public async Task AddArangoDb_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ArangoDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseConfiguration_TimeoutMinusTwo_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ArangoDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseConfiguration_TransportAddressEmpty_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ArangoDb:TestNoValues:Mode", $"{ArangoDbClientCreationMode.Internal}" },
                    { "HealthChecks:ArangoDb:TestNoValues:TransportAddress", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseConfiguration_UsernameNullWithPassword_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ArangoDb:TestNoValues:Mode", $"{ArangoDbClientCreationMode.Internal}" },
                    { "HealthChecks:ArangoDb:TestNoValues:BaseAddress", "base-address" },
                    { "HealthChecks:ArangoDb:TestNoValues:Password", "password" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddArangoDb_UseConfiguration_PasswordNullWithUsername_ThrowsException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:ArangoDb:TestNoValues:Mode", $"{ArangoDbClientCreationMode.Internal}" },
                    { "HealthChecks:ArangoDb:TestNoValues:BaseAddress", "base-address" },
                    { "HealthChecks:ArangoDb:TestNoValues:Username", "username" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
