namespace NetEvolve.HealthChecks.Tests.Integration.Ollama;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Ollama;
using OllamaSharp;

[TestGroup(nameof(Ollama))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<OllamaContainer>]
public class OllamaHealthCheckTests : HealthCheckTestBase
{
    private readonly OllamaContainer _container;

    public OllamaHealthCheckTests(OllamaContainer container) => _container = container;

    [Test]
    public async Task AddOllama_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOllama(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Uri = new Uri(_container.BaseAddress);
                        options.Timeout = 10000; // Set a reasonable timeout
                        options.ClientMode = ClientMode.ServiceUrl;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddOllama_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOllama(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Uri = new Uri(_container.BaseAddress);
                        options.Timeout = 0;
                        options.ClientMode = ClientMode.ServiceUrl;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddOllama_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOllama("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Ollama:TestContainerHealthy:Uri", _container.BaseAddress },
                    { "HealthChecks:Ollama:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:Ollama:TestContainerHealthy:ClientMode", nameof(ClientMode.ServiceUrl) },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOllama_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOllama("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Ollama:TestContainerDegraded:Uri", _container.BaseAddress },
                    { "HealthChecks:Ollama:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:Ollama:TestContainerDegraded:ClientMode", nameof(ClientMode.ServiceUrl) },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOllama_UseConfiguration_UriEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOllama("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?> { { "HealthChecks:Ollama:TestNoValues:Uri", "" } };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOllama_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOllama("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Ollama:TestNoValues:Uri", _container.BaseAddress },
                    { "HealthChecks:Ollama:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddOllama_UseOptionsWithServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOllama(
                    "TestContainerServiceProviderHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.ClientMode = ClientMode.ServiceProvider;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddSingleton<OllamaApiClient>(_ => new OllamaApiClient(new Uri(_container.BaseAddress)))
        );

    [Test]
    public async Task AddOllama_UseOptionsWithKeyedService_Healthy()
    {
        const string serviceKey = "ollama-test-key";

        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOllama(
                    "TestContainerKeyedServiceHealthy",
                    options =>
                    {
                        options.KeyedService = serviceKey;
                        options.Timeout = 10000;
                        options.ClientMode = ClientMode.ServiceProvider;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddKeyedSingleton(serviceKey, (_, _) => new OllamaApiClient(new Uri(_container.BaseAddress)))
        );
    }

    [Test]
    public async Task AddOllama_UseOptionsWithServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddOllama(
                    "TestContainerServiceProviderDegraded",
                    options =>
                    {
                        options.Timeout = 0;
                        options.ClientMode = ClientMode.ServiceProvider;
                    }
                );
            },
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddSingleton<OllamaApiClient>(_ => new OllamaApiClient(new Uri(_container.BaseAddress)))
        );

    [Test]
    public async Task AddOllama_UseConfigurationWithServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOllama("TestContainerServiceProviderHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Ollama:TestContainerServiceProviderHealthy:Timeout", "10000" },
                    {
                        "HealthChecks:Ollama:TestContainerServiceProviderHealthy:ClientMode",
                        nameof(ClientMode.ServiceProvider)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddSingleton<OllamaApiClient>(_ => new OllamaApiClient(new Uri(_container.BaseAddress)))
        );

    [Test]
    public async Task AddOllama_UseConfigurationWithKeyedService_Healthy()
    {
        const string serviceKey = "config-ollama-test-key";

        await RunAndVerify(
            healthChecks => healthChecks.AddOllama("TestContainerKeyedServiceHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Ollama:TestContainerKeyedServiceHealthy:KeyedService", serviceKey },
                    { "HealthChecks:Ollama:TestContainerKeyedServiceHealthy:Timeout", "10000" },
                    {
                        "HealthChecks:Ollama:TestContainerKeyedServiceHealthy:ClientMode",
                        nameof(ClientMode.ServiceProvider)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddKeyedSingleton(serviceKey, (_, _) => new OllamaApiClient(new Uri(_container.BaseAddress)))
        );
    }

    [Test]
    public async Task AddOllama_UseConfigurationWithServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddOllama("TestContainerServiceProviderDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Ollama:TestContainerServiceProviderDegraded:Timeout", "0" },
                    {
                        "HealthChecks:Ollama:TestContainerServiceProviderDegraded:ClientMode",
                        nameof(ClientMode.ServiceProvider)
                    },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddSingleton<OllamaApiClient>(_ => new OllamaApiClient(new Uri(_container.BaseAddress)))
        );
}
