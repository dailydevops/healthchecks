namespace NetEvolve.HealthChecks.Tests.Integration.Keycloak;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Keycloak.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Keycloak;
using NetEvolve.HealthChecks.Tests.Integration.Keycloak.Container;

public abstract class KeycloakHealthCheckBaseTests : HealthCheckTestBase, IAsyncInitializer
{
    private readonly ContainerBase _container;
    private KeycloakClient _client = default!;

    protected KeycloakHealthCheckBaseTests(ContainerBase container) => _container = container;

    public Task InitializeAsync()
    {
        _client = new KeycloakClient(_container.BaseAddress, _container.Username, _container.Password);

        return Task.CompletedTask;
    }

    [Test]
    public async Task AddKeycloak_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeycloak("TestContainerHealthy", options => options.Timeout = 1000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddKeycloak_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddKeycloak(
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

    [Test]
    public async Task AddKeycloak_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddKeycloak("TestContainerHealthy").AddKeycloak("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(_client)
                )
        );

    [Test]
    public async Task AddKeycloak_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddKeycloak(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);
                            return await KeycloakHealthCheck
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
    public async Task AddKeycloak_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKeycloak(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.CommandAsync = async (_, _) =>
                        {
                            await Task.CompletedTask;
                            throw new InvalidOperationException("Unhealthy test exception");
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddKeycloak_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeycloak("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Keycloak:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddKeycloak_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeycloak("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Keycloak:TestContainerKeyedHealthy:KeyedService", "mongodb-test-config" },
                    { "HealthChecks:Keycloak:TestContainerKeyedHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("mongodb-test-config", (_, _) => _client)
        );

    [Test]
    public async Task AddKeycloak_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeycloak("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Keycloak:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddKeycloak_UseConfigration_ConnectionStringEmpty_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeycloak("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Keycloak:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );

    [Test]
    public async Task AddKeycloak_UseConfigration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKeycloak("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Keycloak:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_client)
        );
}
