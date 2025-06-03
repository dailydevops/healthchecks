namespace NetEvolve.HealthChecks.Tests.Integration.AWS.SNS;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.SNS;

[TestGroup($"{nameof(AWS)}.{nameof(SNS)}")]
[ClassDataSource<LocalStackInstance>(Shared = SharedType.PerTestSession)]
public class SimpleNotificationServiceHealthCheckTests : HealthCheckTestBase
{
    private readonly LocalStackInstance _instance;

    public SimpleNotificationServiceHealthCheckTests(LocalStackInstance instance) => _instance = instance;

    [Test]
    public async Task AddSimpleNotificationService_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSimpleNotificationService(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TopicName = LocalStackInstance.TopicName;
                        options.Subscription = _instance.Subscription;
                        options.Mode = CreationMode.BasicAuthentication;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddSimpleNotificationService_UseOptionsCreate_WhenSubscriptionInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSimpleNotificationService(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TopicName = LocalStackInstance.TopicName;
                        options.Subscription = "NotFound";
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddSimpleNotificationService_UseOptionsCreate_WhenTopicInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSimpleNotificationService(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TopicName = "Invalid";
                        options.Subscription = _instance.Subscription;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddSimpleNotificationService_UseOptionsCreate_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSimpleNotificationService(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TopicName = LocalStackInstance.TopicName;
                        options.Subscription = _instance.Subscription;
                        options.Timeout = 0;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddSimpleNotificationService_Run101Subscriptions_Healthy()
    {
        const string topicName = "MassOf101Subscriptions";
        await using var subcription = await _instance.CreateNumberOfSubscriptions(topicName, 101);

        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddSimpleNotificationService(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.AccessKey = LocalStackInstance.AccessKey;
                        options.SecretKey = LocalStackInstance.SecretKey;
                        options.ServiceUrl = _instance.ConnectionString;
                        options.TopicName = topicName;
                        options.Subscription = subcription.SubscriptionArn;
                        options.Mode = CreationMode.BasicAuthentication;
                    }
                );
            },
            HealthStatus.Healthy
        );
    }

    // Configuration-based tests

    [Test]
    public async Task AddSimpleNotificationService_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSimpleNotificationService("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSSNS:TestContainerHealthy:AccessKey", LocalStackInstance.AccessKey },
                    { "HealthChecks:AWSSNS:TestContainerHealthy:SecretKey", LocalStackInstance.SecretKey },
                    { "HealthChecks:AWSSNS:TestContainerHealthy:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSSNS:TestContainerHealthy:TopicName", LocalStackInstance.TopicName },
                    { "HealthChecks:AWSSNS:TestContainerHealthy:Subscription", _instance.Subscription },
                    { "HealthChecks:AWSSNS:TestContainerHealthy:Mode", nameof(CreationMode.BasicAuthentication) },
                    { "HealthChecks:AWSSNS:TestContainerHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSimpleNotificationService_UseConfiguration_WhenSubscriptionInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSimpleNotificationService("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:AccessKey", LocalStackInstance.AccessKey },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:SecretKey", LocalStackInstance.SecretKey },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:TopicName", LocalStackInstance.TopicName },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:Subscription", "NotFound" },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:Mode", nameof(CreationMode.BasicAuthentication) },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSimpleNotificationService_UseConfiguration_WhenTopicInvalid_Unhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSimpleNotificationService("TestContainerUnhealthy"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:AccessKey", LocalStackInstance.AccessKey },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:SecretKey", LocalStackInstance.SecretKey },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:TopicName", "Invalid" },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:Subscription", _instance.Subscription },
                    { "HealthChecks:AWSSNS:TestContainerUnhealthy:Mode", nameof(CreationMode.BasicAuthentication) },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSimpleNotificationService_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddSimpleNotificationService("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:AWSSNS:TestContainerDegraded:AccessKey", LocalStackInstance.AccessKey },
                    { "HealthChecks:AWSSNS:TestContainerDegraded:SecretKey", LocalStackInstance.SecretKey },
                    { "HealthChecks:AWSSNS:TestContainerDegraded:ServiceUrl", _instance.ConnectionString },
                    { "HealthChecks:AWSSNS:TestContainerDegraded:TopicName", LocalStackInstance.TopicName },
                    { "HealthChecks:AWSSNS:TestContainerDegraded:Subscription", _instance.Subscription },
                    { "HealthChecks:AWSSNS:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:AWSSNS:TestContainerDegraded:Mode", nameof(CreationMode.BasicAuthentication) },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddSimpleNotificationService_UseConfiguration_Run101Subscriptions_Healthy()
    {
        const string topicName = "MassOf101SubscriptionsConfig";
        await using (var subscription = await _instance.CreateNumberOfSubscriptions(topicName, 101))
        {
            await RunAndVerify(
                healthChecks => healthChecks.AddSimpleNotificationService("TestContainerHealthy"),
                HealthStatus.Healthy,
                config =>
                {
                    var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                    {
                        { "HealthChecks:AWSSNS:TestContainerHealthy:AccessKey", LocalStackInstance.AccessKey },
                        { "HealthChecks:AWSSNS:TestContainerHealthy:SecretKey", LocalStackInstance.SecretKey },
                        { "HealthChecks:AWSSNS:TestContainerHealthy:ServiceUrl", _instance.ConnectionString },
                        { "HealthChecks:AWSSNS:TestContainerHealthy:TopicName", topicName },
                        { "HealthChecks:AWSSNS:TestContainerHealthy:Subscription", subscription.SubscriptionArn },
                        { "HealthChecks:AWSSNS:TestContainerHealthy:Mode", nameof(CreationMode.BasicAuthentication) },
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            );
        }
    }
}
