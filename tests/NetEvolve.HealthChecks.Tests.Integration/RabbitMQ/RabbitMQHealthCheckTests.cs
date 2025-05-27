namespace NetEvolve.HealthChecks.Tests.Integration.RabbitMQ;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public async Task AddRabbitMQ_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerHealthy", options => options.Timeout = 1000),
            serviceBuilder: RegisterRabbitMQConnection
        );

    [Fact]
    public async Task AddRabbitMQ_UseOptionsWithKeyedService_ShouldReturnHealthy() =>
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
            serviceBuilder: RegisterRabbitMQKeyedConnection("rabbitmq-test")
        );

    [Fact]
    public async Task AddRabbitMQ_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRabbitMQ("TestContainerDegraded", options => options.Timeout = 0),
            serviceBuilder: RegisterRabbitMQConnection
        );

    [Fact]
    public async Task AddRabbitMQ_UseConfiguration_ShouldReturnHealthy() =>
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
            serviceBuilder: RegisterRabbitMQConnection
        );

    [Fact]
    public async Task AddRabbitMQ_UseConfigurationWithKeyedService_ShouldReturnHealthy() =>
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
            serviceBuilder: RegisterRabbitMQKeyedConnection("rabbitmq-test-config")
        );

    [Fact]
    public async Task AddRabbitMQ_UseConfiguration_ShouldReturnDegraded() =>
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
            serviceBuilder: RegisterRabbitMQConnection
        );

    private void RegisterRabbitMQConnection(IServiceCollection services) =>
        services.AddSingleton<global::RabbitMQ.Client.IConnection>(_ =>
        {
            var factory = new global::RabbitMQ.Client.ConnectionFactory { Uri = _container.ConnectionString };
            return factory.CreateConnectionAsync().Result;
        });

    private Action<IServiceCollection> RegisterRabbitMQKeyedConnection(string key) =>
        services =>
        {
            _ = services.AddKeyedSingleton<global::RabbitMQ.Client.IConnection>(
                key,
                (_, _) =>
                {
                    var factory = new global::RabbitMQ.Client.ConnectionFactory { Uri = _container.ConnectionString };
                    return factory.CreateConnectionAsync().Result;
                }
            );
        };
}
