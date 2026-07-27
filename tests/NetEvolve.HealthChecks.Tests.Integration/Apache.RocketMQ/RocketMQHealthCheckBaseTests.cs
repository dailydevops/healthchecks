namespace NetEvolve.HealthChecks.Tests.Integration.Apache.RocketMQ;

using System;
using System.Linq;
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
                        options.EnableSsl = true;
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
            HealthStatus.Unhealthy,
            clearJToken: ClearConnectionRefusedMessages
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
                        options.EnableSsl = true;
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
                    { "HealthChecks:RocketMQ:TestContainerHealthy:EnableSsl", "true" },
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
            },
            clearJToken: ClearConnectionRefusedMessages
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
                    { "HealthChecks:RocketMQ:TestContainerDegraded:EnableSsl", "true" },
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

    // The connection-refused socket/HTTP error text is supplied by the OS and localized to its UI
    // language, so it differs between machines (e.g. German Windows vs. English Linux CI). Scrub it
    // out before verification, matching the pattern used by TableClientAvailableHealthCheckTests.
    private static Argon.JToken? ClearConnectionRefusedMessages(Argon.JToken? token)
    {
        if (token is null)
        {
            return null;
        }

        if (
            token.Value<string>("status") is string status
            && status.Equals(nameof(HealthStatus.Unhealthy), StringComparison.OrdinalIgnoreCase)
        )
        {
            var exception = token["results"]?.FirstOrDefault()?["exception"];

            if (exception is not null)
            {
                exception["message"] = null;

                if (exception["innerExceptions"] is Argon.JArray innerExceptions)
                {
                    foreach (var inner in innerExceptions)
                    {
                        inner["message"] = null;
                    }
                }
            }
        }

        return token;
    }
}
