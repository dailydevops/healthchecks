namespace NetEvolve.HealthChecks.Tests.Integration.AWS;

using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.LocalStack;
using TestContainer = Testcontainers.LocalStack.LocalStackContainer;

public sealed class LocalStackInstance : IAsyncInitializer, IAsyncDisposable
{
    /// <summary>Access Key</summary>
    /// <see href="https://docs.aws.amazon.com/STS/latest/APIReference/API_GetAccessKeyInfo.html" />
    internal const string AccessKey = "AKIAIOSFODNN7EXAMPLE";
    internal const string SecretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
    internal const string BucketName = "test-bucket";
    internal const string TopicName = "test-topic";
    internal const string QueueName = "test-queue";
    internal const string TableName = "test-table";

    private readonly TestContainer _container = new LocalStackBuilder()
        .WithLogger(NullLogger.Instance)
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

        await Task.WhenAll(
                CreateEC2DEfaults(cancellationToken),
                CreateSNSDefaults(cancellationToken),
                CreateSQSDefaults(cancellationToken),
                CreateS3Defaults(cancellationToken),
                CreateDynamoDBDefaults(cancellationToken)
            )
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

    private async Task CreateS3Defaults(CancellationToken cancellationToken)
    {
        // Create S3 Bucket
        using var s3Client = new AmazonS3Client(
            AccessKey,
            SecretKey,
            new AmazonS3Config { ServiceURL = ConnectionString, ForcePathStyle = true }
        );

        var putBucketRequest = new PutBucketRequest { BucketName = BucketName };
        _ = await s3Client.PutBucketAsync(putBucketRequest, cancellationToken).ConfigureAwait(false);
    }

    private async Task CreateEC2DEfaults(CancellationToken cancellationToken)
    {
        // Create EC2 Defaults if needed
        using var ec2Client = new AmazonEC2Client(
            AccessKey,
            SecretKey,
            new AmazonEC2Config { ServiceURL = ConnectionString }
        );

        var request = new RunInstancesRequest()
        {
            InstanceType = InstanceType.T1Micro,
            MinCount = 1,
            MaxCount = 1,
            KeyName = "development",
        };
        _ = await ec2Client.RunInstancesAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task CreateDynamoDBDefaults(CancellationToken cancellationToken)
    {
        // Create DynamoDB Table
        using var dynamoDbClient = new AmazonDynamoDBClient(
            AccessKey,
            SecretKey,
            new AmazonDynamoDBConfig { ServiceURL = ConnectionString }
        );

        var createTableRequest = new CreateTableRequest
        {
            TableName = TableName,
            KeySchema =
            [
                new KeySchemaElement { AttributeName = "Id", KeyType = KeyType.HASH },
            ],
            AttributeDefinitions =
            [
                new AttributeDefinition { AttributeName = "Id", AttributeType = ScalarAttributeType.S },
            ],
            BillingMode = BillingMode.PAY_PER_REQUEST,
        };
        _ = await dynamoDbClient.CreateTableAsync(createTableRequest, cancellationToken).ConfigureAwait(false);
    }
}
