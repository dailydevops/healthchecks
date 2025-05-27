namespace NetEvolve.HealthChecks.Tests.Integration.RabbitMQ;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.RabbitMQ;
using Xunit;

[SetCulture("", asHiddenCategory: true)]
[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQHealthCheckTests : HealthCheckTestBase, IClassFixture<RabbitMQContainer>
{
    private readonly RabbitMQContainer _container;

    public RabbitMQHealthCheckTests(RabbitMQContainer container) => _container = container;

    [Fact]
    public async Task AddRabbitMQ_UseOptions_ShouldReturnHealthy()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerHealthy", options => options.Timeout = 1000),
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Fact]
    public async Task AddRabbitMQ_UseOptionsWithKeyedService_ShouldReturnHealthy()
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
                        options.Timeout = 1000;
                    }
                ),
            serviceBuilder: services => services.AddKeyedSingleton("rabbitmq-test", (_, _) => connection)
        );
    }

    [Fact]
    public async Task AddRabbitMQ_UseOptions_ShouldReturnDegraded()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerDegraded", options => options.Timeout = 0),
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Fact]
    public async Task AddRabbitMQ_UseConfiguration_ShouldReturnHealthy()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RabbitMQ:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(connection)
        );
    }

    [Fact]
    public async Task AddRabbitMQ_UseConfigurationWithKeyedService_ShouldReturnHealthy()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerKeyedHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:RabbitMQ:TestContainerKeyedHealthy:KeyedService", "rabbitmq-test-config" },
                    { "HealthChecks:RabbitMQ:TestContainerKeyedHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("rabbitmq-test-config", (_, _) => connection)
        );
    }

    [Fact]
    public async Task AddRabbitMQ_UseConfiguration_ShouldReturnDegraded()
    {
        var factory = new ConnectionFactory { Uri = _container.ConnectionString };
        var connection = await factory.CreateConnectionAsync();

        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerDegraded"),
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
