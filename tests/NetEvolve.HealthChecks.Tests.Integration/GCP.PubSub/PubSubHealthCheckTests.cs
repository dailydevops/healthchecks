namespace NetEvolve.HealthChecks.Tests.Integration.GCP.PubSub;

using System;
using System.Collections.Generic;
using System.Threading;
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
    public async Task AddPubSub_UseOptions_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddPubSub(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.ProjectName = PubSubEmulator.ProjectId;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddPubSub_UseOptions_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddPubSub(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Timeout = 0;
                        options.ProjectName = PubSubEmulator.ProjectId;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddPubSub_UseOptionsWithKeyedService_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddPubSub(
                    "TestContainerKeyedServiceHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.KeyedService = "pubsub";
                        options.ProjectName = PubSubEmulator.ProjectId;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services => _ = services.AddKeyedSingleton("pubsub", (_, _) => _emulator.Client),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddPubSub_UseConfiguration_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:PubSub:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:GCP:PubSub:TestContainerHealthy:ProjectName", PubSubEmulator.ProjectId },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddPubSub_UseConfiguration_Degraded(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:PubSub:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:GCP:PubSub:TestContainerDegraded:ProjectName", PubSubEmulator.ProjectId },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddPubSub_UseConfiguration_TimeoutMinusTwo_ThrowException(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks => healthChecks.AddPubSub("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:PubSub:TestNoValues:Timeout", "-2" },
                    { "HealthChecks:GCP:PubSub:TestNoValues:ProjectName", PubSubEmulator.ProjectId },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => _ = services.AddSingleton(_ => _emulator.Client),
            cancellationToken: cancellationToken
        );
}
