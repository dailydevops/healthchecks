namespace NetEvolve.HealthChecks.Tests.Integration.RabbitMQ;

using System.Collections.Generic;
using System.Threading.Tasks;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RabbitMQ;

[TestGroup(nameof(RabbitMQ))]
[ClassDataSource<RabbitMQContainer>(Shared = InstanceSharedType.RabbitMQ)]
public sealed class RabbitMQHealthCheckTests : HealthCheckTestBase
{
    private readonly RabbitMQContainer _container;

    public RabbitMQHealthCheckTests(RabbitMQContainer container) => _container = container;

    [Test]
    public async Task AddRabbitMQ_UseOptions_Healthy()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseOptionsWithKeyedService_Healthy()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddRabbitMQ(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "rabbitmq-test";
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("rabbitmq-test", (_, _) => connection)
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseOptions_Degraded()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseConfiguration_Healthy()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RabbitMQ:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseConfigurationWithKeyedService_Healthy()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RabbitMQ:TestContainerKeyedHealthy:KeyedService", "rabbitmq-test-config" },
                    { "HealthChecks:RabbitMQ:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("rabbitmq-test-config", (_, _) => connection)
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseConfiguration_Degraded()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RabbitMQ:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }
}
