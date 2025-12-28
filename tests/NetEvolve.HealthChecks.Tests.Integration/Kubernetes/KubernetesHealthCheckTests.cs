namespace NetEvolve.HealthChecks.Tests.Integration.Kubernetes;

using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Kubernetes;

[TestGroup(nameof(Kubernetes))]
[TestGroup("Z04TestGroup")]
[ClassDataSource<K3sDatabase>(Shared = SharedType.PerClass)]
public sealed class KubernetesHealthCheckTests : HealthCheckTestBase
{
    private readonly K3sDatabase _database;

    public KubernetesHealthCheckTests(K3sDatabase database) => _database = database;

    [Test]
    public async Task AddKubernetes_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKubernetes("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(_ => _database.CreateClient())
        );

    [Test]
    public async Task AddKubernetes_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKubernetes("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(_ => _database.CreateClient())
        );

    [Test]
    public async Task AddKubernetes_UseOptions_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddKubernetes(
                    "TestContainerHealthyKeyed",
                    options =>
                    {
                        options.KeyedService = "kubernetes-test";
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                _ = services.AddKeyedSingleton("kubernetes-test", (_, _) => _database.CreateClient())
        );

    [Test]
    public async Task AddKubernetes_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKubernetes("TestContainerConfigHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Kubernetes:TestContainerConfigHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_ => _database.CreateClient())
        );

    [Test]
    public async Task AddKubernetes_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKubernetes("TestContainerConfigDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Kubernetes:TestContainerConfigDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(_ => _database.CreateClient())
        );

    [Test]
    public async Task AddKubernetes_UseConfiguration_WithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddKubernetes("TestContainerConfigKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Kubernetes:TestContainerConfigKeyedHealthy:Timeout", "10000" },
                    { "HealthChecks:Kubernetes:TestContainerConfigKeyedHealthy:KeyedService", "kubernetes-keyed-test" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                _ = services.AddKeyedSingleton("kubernetes-keyed-test", (_, _) => _database.CreateClient())
        );
}
