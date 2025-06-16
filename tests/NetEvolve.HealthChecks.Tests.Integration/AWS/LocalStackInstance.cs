namespace NetEvolve.HealthChecks.Tests.Integration.AWS;

using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Testcontainers.LocalStack;
using TestContainer = Testcontainers.LocalStack.LocalStackContainer;

public sealed class LocalStackInstance : IAsyncInitializer, IAsyncDisposable
{
    /// <summary>Access Key</summary>
    /// <see href="https://docs.aws.amazon.com/STS/latest/APIReference/API_GetAccessKeyInfo.html" />
    internal const string AccessKey = "AKIAIOSFODNN7EXAMPLE";
    internal const string SecretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
    internal const string TopicName = "TestTopic";
    internal const string QueueName = "MuhKuh";

    private readonly TestContainer _container = new LocalStackBuilder()
        .WithImage("localstack/localstack:stable")
        .WithEnvironment("AWS_ACCESS_KEY_ID", AccessKey)
        .WithEnvironment("AWS_SECRET_ACCESS_KEY", SecretKey)
        .Build();

    internal string ConnectionString => _container.GetConnectionString();

    internal string Subscription { get; private set; } = default!;

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        var cancellationToken = TestContext.Current!.CancellationToken;

        await _container.StartAsync(cancellationToken).ConfigureAwait(false);

        await Task.WhenAll(CreateSNSDefaults(cancellationToken), CreateSQSDefaults(cancellationToken))
            .ConfigureAwait(false);
    }

    internal async Task<DisposableSubscription> CreateNumberOfSubscriptions(string topicName, int numberOfSubscriptions)
    {
        var client = new AmazonSimpleNotificationServiceClient(
            AccessKey,
            SecretKey,
            new AmazonSimpleNotificationServiceConfig { ServiceURL = ConnectionString }
        );
        var topic = await client.CreateTopicAsync(topicName).ConfigureAwait(false);

        var subscription = await client.SubscribeAsync(topic.TopicArn, "email", $"Test{1:D6}@example.com");

        if (numberOfSubscriptions > 1)
        {
            for (var i = 2; i <= numberOfSubscriptions; i++)
            {
                _ = await client.SubscribeAsync(topic.TopicArn, "email", $"Test{i:D6}@example.com");
            }
        }

        return new DisposableSubscription(client, topic.TopicArn, subscription.SubscriptionArn);
    }

    private async Task CreateSNSDefaults(CancellationToken cancellationToken)
    {
        // Create SNS Topic & Subscription
        using var snsClient = new AmazonSimpleNotificationServiceClient(
            AccessKey,
            SecretKey,
            new AmazonSimpleNotificationServiceConfig { ServiceURL = ConnectionString }
        );

        var topic = await snsClient.CreateTopicAsync(TopicName, cancellationToken).ConfigureAwait(false);
        var subscription = await snsClient.SubscribeAsync(
            topic.TopicArn,
            "email",
            "test@example.com",
            cancellationToken
        );
        Subscription = subscription
            .SubscriptionArn.Replace(topic.TopicArn, string.Empty, StringComparison.Ordinal)
            .Trim(':');
    }

    private async Task CreateSQSDefaults(CancellationToken cancellationToken)
    {
        // Create SQS Queue
        using var sqsClient = new AmazonSQSClient(
            AccessKey,
            SecretKey,
            new AmazonSQSConfig { ServiceURL = ConnectionString }
        );

        _ = await sqsClient.CreateQueueAsync(QueueName, cancellationToken).ConfigureAwait(false);
    }
}
