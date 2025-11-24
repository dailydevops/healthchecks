namespace NetEvolve.HealthChecks.Tests.Integration.Mosquitto;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MQTTnet;
using MQTTnet.Client;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Mosquitto;

[ClassDataSource<MosquittoContainer>]
[TestGroup(nameof(Mosquitto))]
[TestGroup("Z05TestGroup")]
public sealed class MosquittoHealthCheckTests : HealthCheckTestBase
{
    private readonly MosquittoContainer _container;

    public MosquittoHealthCheckTests(MosquittoContainer container) => _container = container;

    [Test]
    public async Task AddMosquitto_UseOptions_Healthy()
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder().WithConnectionUri(_container.ConnectionString).Build();

        _ = await mqttClient.ConnectAsync(options);

        await RunAndVerify(
            healthChecks => healthChecks.AddMosquitto("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(mqttClient)
        );
    }

    [Test]
    public async Task AddMosquitto_UseOptionsWithKeyedService_Healthy()
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder().WithConnectionUri(_container.ConnectionString).Build();

        _ = await mqttClient.ConnectAsync(options);

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddMosquitto(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "mosquitto-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("mosquitto-test", (_, _) => mqttClient)
        );
    }

    [Test]
    public async Task AddMosquitto_UseOptions_Degraded()
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder().WithConnectionUri(_container.ConnectionString).Build();

        _ = await mqttClient.ConnectAsync(options);

        await RunAndVerify(
            healthChecks => healthChecks.AddMosquitto("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(mqttClient)
        );
    }

    [Test]
    public async Task AddMosquitto_UseConfiguration_Healthy()
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder().WithConnectionUri(_container.ConnectionString).Build();

        _ = await mqttClient.ConnectAsync(options);

        await RunAndVerify(
            healthChecks => healthChecks.AddMosquitto("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Mosquitto:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(mqttClient)
        );
    }

    [Test]
    public async Task AddMosquitto_UseConfigurationWithKeyedService_Healthy()
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder().WithConnectionUri(_container.ConnectionString).Build();

        _ = await mqttClient.ConnectAsync(options);

        await RunAndVerify(
            healthChecks => healthChecks.AddMosquitto("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Mosquitto:TestContainerKeyedHealthy:KeyedService", "mosquitto-test-config" },
                    { "HealthChecks:Mosquitto:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("mosquitto-test-config", (_, _) => mqttClient)
        );
    }

    [Test]
    public async Task AddMosquitto_UseConfiguration_Degraded()
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder().WithConnectionUri(_container.ConnectionString).Build();

        _ = await mqttClient.ConnectAsync(options);

        await RunAndVerify(
            healthChecks => healthChecks.AddMosquitto("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:Mosquitto:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(mqttClient)
        );
    }
}
