namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Pulsar;

using System.Collections.Generic;
using System.Threading.Tasks;
using DotPulsar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Pulsar;

[TestGroup($"{nameof(Apache)}.{nameof(Pulsar)}")]
[TestGroup("Z00TestGroup")]
[ClassDataSource<PulsarContainer>(Shared = SharedType.PerTestSession)]
public sealed class PulsarHealthCheckTests : HealthCheckTestBase
{
    private readonly PulsarContainer _container;

    public PulsarHealthCheckTests(PulsarContainer container) => _container = container;

    [Test]
    public async Task AddPulsar_UseOptions_Healthy()
    {
        var client = PulsarClient.Builder().ServiceUrl(_container.ServiceUrl).Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddPulsar("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(client)
        );
    }

    [Test]
    public async Task AddPulsar_UseOptionsWithKeyedService_Healthy()
    {
        var client = PulsarClient.Builder().ServiceUrl(_container.ServiceUrl).Build();

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddPulsar(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "pulsar-test";
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("pulsar-test", (_, _) => client)
        );
    }

    [Test]
    public async Task AddPulsar_UseOptions_Degraded()
    {
        var client = PulsarClient.Builder().ServiceUrl(_container.ServiceUrl).Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddPulsar("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(client)
        );
    }

    [Test]
    public async Task AddPulsar_UseConfiguration_Healthy()
    {
        var client = PulsarClient.Builder().ServiceUrl(_container.ServiceUrl).Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddPulsar("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Pulsar:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(client)
        );
    }

    [Test]
    public async Task AddPulsar_UseConfigurationWithKeyedService_Healthy()
    {
        var client = PulsarClient.Builder().ServiceUrl(_container.ServiceUrl).Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddPulsar("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Pulsar:TestContainerKeyedHealthy:KeyedService", "pulsar-test-config" },
                    { "HealthChecks:Pulsar:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("pulsar-test-config", (_, _) => client)
        );
    }

    [Test]
    public async Task AddPulsar_UseConfiguration_Degraded()
    {
        var client = PulsarClient.Builder().ServiceUrl(_container.ServiceUrl).Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddPulsar("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Pulsar:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(client)
        );
    }
}
