namespace NetEvolve.HealthChecks.Tests.Integration.GCP.PubSub;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.PubSub;

[TestGroup($"GCP.{nameof(PubSub)}")]
[TestGroup("Z03TestGroup")]
[ClassDataSource<PubSubEmulator>(Shared = SharedType.PerClass)]
public sealed class PubSubHealthCheckTests : HealthCheckTestBase
{
    private readonly PubSubEmulator _emulator;

    public PubSubHealthCheckTests(PubSubEmulator emulator) => _emulator = emulator;

    [Test]
    public async Task AddPubSub_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client)
        );

    [Test]
    public async Task AddPubSub_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client)
        );

    [Test]
    public async Task AddPubSub_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPubSub(
                    "TestContainerKeyedServiceHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.KeyedService = "pubsub";
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddKeyedSingleton("pubsub", (_, _) => _emulator.Client)
        );

    [Test]
    public async Task AddPubSub_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:PubSub:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client)
        );

    [Test]
    public async Task AddPubSub_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:PubSub:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client)
        );

    [Test]
    public async Task AddPubSub_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:PubSub:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client)
        );
}
