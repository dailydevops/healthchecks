namespace NetEvolve.HealthChecks.Tests.Integration.IbmMQ;

using System.Collections.Generic;
using System.Threading.Tasks;
using IBM.WMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.IbmMQ;

[TestGroup(nameof(IbmMQ))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<IbmMQContainer>(Shared = SharedType.PerClass)]
public sealed class IbmMQHealthCheckTests : HealthCheckTestBase
{
    private readonly IbmMQContainer _container;

    public IbmMQHealthCheckTests(IbmMQContainer container) => _container = container;

    [Test]
    public async Task AddIbmMQ_UseOptions_Healthy()
    {
        var properties = new Hashtable
        {
            { MQC.HOST_NAME_PROPERTY, _container.Host },
            { MQC.PORT_PROPERTY, _container.Port },
            { MQC.CHANNEL_PROPERTY, _container.Channel },
        };

        var queueManager = new MQQueueManager(_container.QueueManagerName, properties);

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseOptionsWithKeyedService_Healthy()
    {
        var properties = new Hashtable
        {
            { MQC.HOST_NAME_PROPERTY, _container.Host },
            { MQC.PORT_PROPERTY, _container.Port },
            { MQC.CHANNEL_PROPERTY, _container.Channel },
        };

        var queueManager = new MQQueueManager(_container.QueueManagerName, properties);

        await RunAndVerify(
            healthChecks =>
                healthChecks.AddIbmMQ(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "ibmmq-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton("ibmmq-test", (_, _) => queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseOptions_Degraded()
    {
        var properties = new Hashtable
        {
            { MQC.HOST_NAME_PROPERTY, _container.Host },
            { MQC.PORT_PROPERTY, _container.Port },
            { MQC.CHANNEL_PROPERTY, _container.Channel },
        };

        var queueManager = new MQQueueManager(_container.QueueManagerName, properties);

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseConfiguration_Healthy()
    {
        var properties = new Hashtable
        {
            { MQC.HOST_NAME_PROPERTY, _container.Host },
            { MQC.PORT_PROPERTY, _container.Port },
            { MQC.CHANNEL_PROPERTY, _container.Channel },
        };

        var queueManager = new MQQueueManager(_container.QueueManagerName, properties);

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:IbmMQ:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseConfigurationWithKeyedService_Healthy()
    {
        var properties = new Hashtable
        {
            { MQC.HOST_NAME_PROPERTY, _container.Host },
            { MQC.PORT_PROPERTY, _container.Port },
            { MQC.CHANNEL_PROPERTY, _container.Channel },
        };

        var queueManager = new MQQueueManager(_container.QueueManagerName, properties);

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:IbmMQ:TestContainerKeyedHealthy:KeyedService", "ibmmq-test-config" },
                    { "HealthChecks:IbmMQ:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddKeyedSingleton("ibmmq-test-config", (_, _) => queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseConfiguration_Degraded()
    {
        var properties = new Hashtable
        {
            { MQC.HOST_NAME_PROPERTY, _container.Host },
            { MQC.PORT_PROPERTY, _container.Port },
            { MQC.CHANNEL_PROPERTY, _container.Channel },
        };

        var queueManager = new MQQueueManager(_container.QueueManagerName, properties);

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:IbmMQ:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton(queueManager)
        );
    }
}
