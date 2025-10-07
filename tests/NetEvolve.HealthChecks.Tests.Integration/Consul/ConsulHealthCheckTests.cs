namespace NetEvolve.HealthChecks.Tests.Integration.Consul;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Consul;

[TestGroup(nameof(Consul))]
[ClassDataSource<ConsulDatabase>(Shared = InstanceSharedType.Consul)]
public sealed class ConsulHealthCheckTests : HealthCheckTestBase
{
    private readonly ConsulDatabase _database;

    public ConsulHealthCheckTests(ConsulDatabase database) => _database = database;

    [Test]
    public async Task AddConsul_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddConsul("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddSingleton<IConsulClient>(
                    _ => new ConsulClient(config => config.Address = new Uri(_database.HttpEndpoint))
                )
        );

    [Test]
    public async Task AddConsul_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddConsul("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services =>
                services.AddSingleton<IConsulClient>(
                    _ => new ConsulClient(config => config.Address = new Uri(_database.HttpEndpoint))
                )
        );

    [Test]
    public async Task AddConsul_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddConsul(
                    "TestContainerHealthyKeyed",
                    options =>
                    {
                        options.KeyedService = "consul-test";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                _ = services.AddKeyedSingleton<IConsulClient>(
                    "consul-test",
                    (_, _) => new ConsulClient(config => config.Address = new Uri(_database.HttpEndpoint))
                );
            }
        );

    [Test]
    public async Task AddConsul_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddConsul("TestContainerConfigHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Consul:TestContainerConfigHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddSingleton<IConsulClient>(
                    _ => new ConsulClient(config => config.Address = new Uri(_database.HttpEndpoint))
                )
        );

    [Test]
    public async Task AddConsul_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddConsul("TestContainerConfigDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Consul:TestContainerConfigDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddSingleton<IConsulClient>(
                    _ => new ConsulClient(config => config.Address = new Uri(_database.HttpEndpoint))
                )
        );

    [Test]
    public async Task AddConsul_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddConsul("TestContainerConfigKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Consul:TestContainerConfigKeyedHealthy:Timeout", "10000" },
                    { "HealthChecks:Consul:TestContainerConfigKeyedHealthy:KeyedService", "consul-keyed-test" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                _ = services.AddKeyedSingleton<IConsulClient>(
                    "consul-keyed-test",
                    (_, _) => new ConsulClient(config => config.Address = new Uri(_database.HttpEndpoint))
                );
            }
        );
}
