namespace NetEvolve.HealthChecks.Tests.Integration.Ollama;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Ollama;

[TestGroup(nameof(Ollama))]
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
                        options.Uri = new System.Uri(_container.BaseAddress);
                        options.Timeout = 10000; // Set a reasonable timeout
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
                        options.Uri = new System.Uri(_container.BaseAddress);
                        options.Timeout = 0;
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
}
