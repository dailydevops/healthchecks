namespace NetEvolve.HealthChecks.Tests.Integration.AWS.SNS;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.AWS.SNS;

[TestGroup($"{nameof(AWS)}.{nameof(SNS)}")]
public class SimpleNotificationServiceHealthCheckTests : HealthCheckTestBase, IClassFixture<LocalStackInstance>
{
    private readonly LocalStackInstance _instance;

    public SimpleNotificationServiceHealthCheckTests(LocalStackInstance instance) => _instance = instance;

    [Fact]
    public async Task AddSimpleNotificationService_UseOptionsCreate_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
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
                }
            );
        });

    [Fact]
    public async Task AddSimpleNotificationService_UseOptionsCreate_WhenSubscriptionInvalid_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddSimpleNotificationService_UseOptionsCreate_WhenTopicInvalid_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddSimpleNotificationService_UseOptionsCreate_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
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
        });

    [Fact]
    public async Task AddSimpleNotificationService_Run101Subscriptions_ShouldReturnHealthy()
    {
        const string topicName = "MassOf101Subscriptions";
        await using (var subcription = await _instance.CreateNumberOfSubscriptions(topicName, 101))
        {
            await RunAndVerify(healthChecks =>
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
            });
        }
    }
}
