namespace NetEvolve.HealthChecks.Tests.Integration.RabbitMQ;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.RabbitMQ;

[TestGroup(nameof(RabbitMQ))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<RabbitMQContainer>(Shared = SharedType.PerClass)]
public sealed class RabbitMQHealthCheckTests : HealthCheckTestBase
{
    private readonly RabbitMQContainer _container;

    public RabbitMQHealthCheckTests(RabbitMQContainer container) => _container = container;

    [Test]
    public async Task AddRabbitMQ_UseOptions_Healthy(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync(cancellationToken);

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(connection),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseOptionsWithKeyedService_Healthy(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync(cancellationToken);

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
            serviceBuilder: services => services.AddKeyedSingleton("rabbitmq-test", (_, _) => connection),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseOptions_Degraded(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync(cancellationToken);

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(connection),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseConfiguration_Healthy(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync(cancellationToken);

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
            serviceBuilder: services => services.AddSingleton(connection),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseConfigurationWithKeyedService_Healthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync(cancellationToken);

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
            serviceBuilder: services => services.AddKeyedSingleton("rabbitmq-test-config", (_, _) => connection),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddRabbitMQ_UseConfiguration_Degraded(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync(cancellationToken);

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
            serviceBuilder: services => services.AddSingleton(connection),
            cancellationToken: cancellationToken
        );
    }
}
