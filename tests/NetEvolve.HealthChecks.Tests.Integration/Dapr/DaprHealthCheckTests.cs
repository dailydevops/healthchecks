namespace NetEvolve.HealthChecks.Tests.Integration.Dapr;

using System.Collections.Generic;
using System.Threading.Tasks;
using global::Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Dapr;

[TestGroup(nameof(Dapr))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<DaprContainer>(Shared = SharedType.PerTestSession)]
public sealed class DaprHealthCheckTests : HealthCheckTestBase
{
    private readonly DaprContainer _container;

    public DaprHealthCheckTests(DaprContainer container) => _container = container;

    [Test]
    public async Task AddDapr_UseOptions_Healthy()
    {
        var clientBuilder = new DaprClientBuilder()
            .UseHttpEndpoint(_container.HttpEndpoint)
            .UseGrpcEndpoint(_container.GrpcEndpoint);
        var client = clientBuilder.Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddDapr(options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(client)
        );
    }

    [Test]
    public async Task AddDapr_UseOptionsWithKeyedService_Healthy()
    {
        var clientBuilder = new DaprClientBuilder()
            .UseHttpEndpoint(_container.HttpEndpoint)
            .UseGrpcEndpoint(_container.GrpcEndpoint);
        var client = clientBuilder.Build();

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddDapr(
                    options =>
                    {
                        options.KeyedService = "dapr-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("dapr-test", (_, _) => client)
        );
    }

    [Test]
    public async Task AddDapr_UseOptions_Degraded()
    {
        var clientBuilder = new DaprClientBuilder()
            .UseHttpEndpoint(_container.HttpEndpoint)
            .UseGrpcEndpoint(_container.GrpcEndpoint);
        var client = clientBuilder.Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddDapr(options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(client)
        );
    }

    [Test]
    public async Task AddDapr_UseConfiguration_Healthy()
    {
        var clientBuilder = new DaprClientBuilder()
            .UseHttpEndpoint(_container.HttpEndpoint)
            .UseGrpcEndpoint(_container.GrpcEndpoint);
        var client = clientBuilder.Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddDapr(),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Dapr:DaprSidecar:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(client)
        );
    }

    [Test]
    public async Task AddDapr_UseConfigurationWithKeyedService_Healthy()
    {
        var clientBuilder = new DaprClientBuilder()
            .UseHttpEndpoint(_container.HttpEndpoint)
            .UseGrpcEndpoint(_container.GrpcEndpoint);
        var client = clientBuilder.Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddDapr(),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Dapr:DaprSidecar:KeyedService", "dapr-test-config" },
                    { "HealthChecks:Dapr:DaprSidecar:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("dapr-test-config", (_, _) => client)
        );
    }

    [Test]
    public async Task AddDapr_UseConfiguration_Degraded()
    {
        var clientBuilder = new DaprClientBuilder()
            .UseHttpEndpoint(_container.HttpEndpoint)
            .UseGrpcEndpoint(_container.GrpcEndpoint);
        var client = clientBuilder.Build();

        await RunAndVerify(
            healthChecks => healthChecks.AddDapr(),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Dapr:DaprSidecar:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(client)
        );
    }
}
