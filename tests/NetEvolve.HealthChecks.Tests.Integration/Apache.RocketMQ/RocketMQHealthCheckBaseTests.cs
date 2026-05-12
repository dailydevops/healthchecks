namespace NetEvolve.HealthChecks.Tests.Integration.Apache.RocketMQ;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Apache.RocketMQ;

public abstract class RocketMQHealthCheckBaseTests : HealthCheckTestBase
{
    private readonly IRocketMQAccessor _accessor;

    protected RocketMQHealthCheckBaseTests(IRocketMQAccessor accessor) => _accessor = accessor;

    [Test]
    public async Task AddRocketMQ_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRocketMQ(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Endpoint = _accessor.Endpoint;
                        options.Topic = _accessor.Topic;
                        options.AccessKey = _accessor.AccessKey;
                        options.AccessSecret = _accessor.AccessSecret;
                        options.EnableSsl = false;
                        options.Timeout = 30000;
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddRocketMQ_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRocketMQ(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Endpoint = "127.0.0.1:9999";
                        options.Topic = _accessor.Topic;
                        options.EnableSsl = false;
                        options.Timeout = 5000;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddRocketMQ_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddRocketMQ(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Endpoint = _accessor.Endpoint;
                        options.Topic = _accessor.Topic;
                        options.AccessKey = _accessor.AccessKey;
                        options.AccessSecret = _accessor.AccessSecret;
                        options.EnableSsl = false;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddRocketMQ_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRocketMQ("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RocketMQ:TestContainerHealthy:Endpoint", _accessor.Endpoint },
                    { "HealthChecks:RocketMQ:TestContainerHealthy:Topic", _accessor.Topic },
                    { "HealthChecks:RocketMQ:TestContainerHealthy:AccessKey", _accessor.AccessKey },
                    { "HealthChecks:RocketMQ:TestContainerHealthy:AccessSecret", _accessor.AccessSecret },
                    { "HealthChecks:RocketMQ:TestContainerHealthy:EnableSsl", "false" },
                    { "HealthChecks:RocketMQ:TestContainerHealthy:Timeout", "30000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddRocketMQ_UseConfiguration_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRocketMQ("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RocketMQ:TestContainerUnhealthy:Endpoint", "127.0.0.1:9999" },
                    { "HealthChecks:RocketMQ:TestContainerUnhealthy:Topic", _accessor.Topic },
                    { "HealthChecks:RocketMQ:TestContainerUnhealthy:EnableSsl", "false" },
                    { "HealthChecks:RocketMQ:TestContainerUnhealthy:Timeout", "5000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddRocketMQ_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRocketMQ("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RocketMQ:TestContainerDegraded:Endpoint", _accessor.Endpoint },
                    { "HealthChecks:RocketMQ:TestContainerDegraded:Topic", _accessor.Topic },
                    { "HealthChecks:RocketMQ:TestContainerDegraded:AccessKey", _accessor.AccessKey },
                    { "HealthChecks:RocketMQ:TestContainerDegraded:AccessSecret", _accessor.AccessSecret },
                    { "HealthChecks:RocketMQ:TestContainerDegraded:EnableSsl", "false" },
                    { "HealthChecks:RocketMQ:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddRocketMQ_UseConfiguration_EndpointNull_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRocketMQ("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RocketMQ:TestNoValues:Endpoint", "" },
                    { "HealthChecks:RocketMQ:TestNoValues:Topic", _accessor.Topic },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddRocketMQ_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddRocketMQ("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:RocketMQ:TestNoValues:Endpoint", _accessor.Endpoint },
                    { "HealthChecks:RocketMQ:TestNoValues:Topic", _accessor.Topic },
                    { "HealthChecks:RocketMQ:TestNoValues:EnableSsl", "false" },
                    { "HealthChecks:RocketMQ:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
