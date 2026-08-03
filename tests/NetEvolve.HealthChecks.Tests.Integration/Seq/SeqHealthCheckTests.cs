namespace NetEvolve.HealthChecks.Tests.Integration.Seq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Seq.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Seq;
using NetEvolve.HealthChecks.Tests.Integration.Seq.Container;

[TestGroup(nameof(Seq))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<SeqContainerAccess>(Shared = SharedType.PerClass)]
public sealed class SeqHealthCheckTests : HealthCheckTestBase
{
    private readonly SeqContainerAccess _container;

    public SeqHealthCheckTests(SeqContainerAccess container) => _container = container;

    [Test]
    public async Task AddSeq_UseOptions_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSeq("ServiceProviderHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(new SeqConnection(_container.ServerUrl.ToString()))
        );

    [Test]
    public async Task AddSeq_UseOptions_WithKeyedService_Healthy()
    {
        const string serviceKey = "seq-test";

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSeq(
                    "KeyedServiceProviderHealthy",
                    options =>
                    {
                        options.KeyedService = serviceKey;
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddKeyedSingleton(serviceKey, (_, _) => new SeqConnection(_container.ServerUrl.ToString()))
        );
    }

    [Test]
    public async Task AddSeq_UseOptions_ModeServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSeq("ServiceProviderDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(new SeqConnection(_container.ServerUrl.ToString()))
        );

    [Test]
    public async Task AddSeq_UseOptions_ModeServerUrl_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSeq(
                    "ServerUrlHealthy",
                    options =>
                    {
                        options.Mode = SeqClientCreationMode.ServerUrl;
                        options.ServerUrl = _container.ServerUrl;
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddSeq_UseOptions_ModeServerUrl_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSeq(
                    "ServerUrlDegraded",
                    options =>
                    {
                        options.Mode = SeqClientCreationMode.ServerUrl;
                        options.ServerUrl = _container.ServerUrl;
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddSeq_UseOptionsDoubleRegistered_ThrowsArgumentException() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks => healthChecks.AddSeq("DoubleRegistered").AddSeq("DoubleRegistered"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton(new SeqConnection(_container.ServerUrl.ToString()))
                )
        );

    [Test]
    public async Task AddSeq_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSeq(
                    "Unhealthy",
                    options =>
                        options.CommandAsync = async (_, _) =>
                        {
                            await Task.CompletedTask;
                            throw new InvalidOperationException("Unhealthy test exception");
                        }
                ),
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(new SeqConnection(_container.ServerUrl.ToString()))
        );

    [Test]
    public async Task AddSeq_UseConfiguration_ModeServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSeq("ConfigServiceProviderHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Seq:ConfigServiceProviderHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(new SeqConnection(_container.ServerUrl.ToString()))
        );

    [Test]
    public async Task AddSeq_UseConfiguration_ModeServerUrl_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSeq("ConfigServerUrlHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Seq:ConfigServerUrlHealthy:Mode", nameof(SeqClientCreationMode.ServerUrl) },
                    { "HealthChecks:Seq:ConfigServerUrlHealthy:ServerUrl", _container.ServerUrl.ToString() },
                    { "HealthChecks:Seq:ConfigServerUrlHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSeq_UseConfiguration_ModeServerUrl_ServerUrlMissing_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSeq("ConfigServerUrlMissing"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Seq:ConfigServerUrlMissing:Mode", nameof(SeqClientCreationMode.ServerUrl) },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSeq_UseConfiguration_TimeoutMinusTwo_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSeq("ConfigTimeoutInvalid"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Seq:ConfigTimeoutInvalid:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(new SeqConnection(_container.ServerUrl.ToString()))
        );
}
