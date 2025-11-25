namespace NetEvolve.HealthChecks.Tests.Integration.NATS;

using System.Collections.Generic;
using System.Threading.Tasks;
using global::NATS.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.NATS;

[TestGroup(nameof(NATS))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<NatsContainer>(Shared = SharedType.PerTestSession)]
public sealed class NATSHealthCheckTests : HealthCheckTestBase
{
    private readonly NatsContainer _container;

    public NATSHealthCheckTests(NatsContainer container) => _container = container;

    [Test]
    public async Task AddNATS_UseOptions_Healthy()
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection(_container.ConnectionString);

        await RunAndVerify(
            healthChecks => healthChecks.AddNats("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Test]
    public async Task AddNATS_UseOptionsWithKeyedService_Healthy()
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection(_container.ConnectionString);

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddNats(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "nats-test";
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("nats-test", (_, _) => connection)
        );
    }

    [Test]
    public async Task AddNATS_UseOptions_Degraded()
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection(_container.ConnectionString);

        await RunAndVerify(
            healthChecks => healthChecks.AddNats("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Test]
    public async Task AddNATS_UseConfiguration_Healthy()
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection(_container.ConnectionString);

        await RunAndVerify(
            healthChecks => healthChecks.AddNats("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:NATS:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Test]
    public async Task AddNATS_UseConfigurationWithKeyedService_Healthy()
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection(_container.ConnectionString);

        await RunAndVerify(
            healthChecks => healthChecks.AddNats("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:NATS:TestContainerKeyedHealthy:KeyedService", "nats-test-config" },
                    { "HealthChecks:NATS:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("nats-test-config", (_, _) => connection)
        );
    }

    [Test]
    public async Task AddNATS_UseConfiguration_Degraded()
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection(_container.ConnectionString);

        await RunAndVerify(
            healthChecks => healthChecks.AddNats("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:NATS:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }
}
