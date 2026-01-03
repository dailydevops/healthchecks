namespace NetEvolve.HealthChecks.Tests.Integration.IbmMQ;

using System.Collections;
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
        using var queueManager = CreateQueueManager(_container.Host);

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton(queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseOptions_AndDisconnected_Unhealthy()
    {
        using var queueManager = CreateQueueManager(_container.Host);

        queueManager.Disconnect();

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton(queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseOptionsWithKeyedService_Healthy()
    {
        using var queueManager = CreateQueueManager(_container.Host);

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
        using var queueManager = CreateQueueManager(_container.Host);

        await RunAndVerify(
            healthChecks => healthChecks.AddIbmMQ("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton(queueManager)
        );
    }

    [Test]
    public async Task AddIbmMQ_UseConfiguration_Healthy()
    {
        using var queueManager = CreateQueueManager(_container.Host);

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
        using var queueManager = CreateQueueManager(_container.Host);

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
        using var queueManager = CreateQueueManager(_container.Host);

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

    private static MQQueueManager CreateQueueManager(string host)
    {
        var properties = new Hashtable
        {
            { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
            { MQC.CHANNEL_PROPERTY, IbmMQContainer.Channel },
            { MQC.CONNECTION_NAME_PROPERTY, host },
            { MQC.USER_ID_PROPERTY, IbmMQContainer.User },
            { MQC.PASSWORD_PROPERTY, IbmMQContainer.Password },
        };

        return new MQQueueManager(IbmMQContainer.QueueManager, properties);
    }
}
