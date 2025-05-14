namespace NetEvolve.HealthChecks.Tests.Integration.AWS;

using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Testcontainers.LocalStack;
using TestContainer = Testcontainers.LocalStack.LocalStackContainer;

public sealed class LocalStackInstance : IAsyncLifetime
{
    /// <summary>Access Key</summary>
    /// <see href="https://docs.aws.amazon.com/STS/latest/APIReference/API_GetAccessKeyInfo.html" />
    internal const string AccessKey = "AKIAIOSFODNN7EXAMPLE";
    internal const string SecretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
    internal const string TopicName = "TestTopic";

    private readonly TestContainer _container = new LocalStackBuilder()
        .WithEnvironment("AWS_ACCESS_KEY_ID", AccessKey)
        .WithEnvironment("AWS_SECRET_ACCESS_KEY", SecretKey)
        .Build();

    internal string ConnectionString => _container.GetConnectionString();

    internal string Subscription { get; private set; } = default!;

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        // Create SNS Topic & Subscription
        using var client = new AmazonSimpleNotificationServiceClient(
            AccessKey,
            SecretKey,
            new AmazonSimpleNotificationServiceConfig { ServiceURL = ConnectionString }
        );

        var topic = await client.CreateTopicAsync(TopicName).ConfigureAwait(false);
        var subscription = await client.SubscribeAsync(topic.TopicArn, "email", "test@example.com");
        Subscription = subscription
            .SubscriptionArn.Replace(topic.TopicArn, string.Empty, StringComparison.Ordinal)
            .Trim(':');
    }
}
