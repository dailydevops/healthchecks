namespace NetEvolve.HealthChecks.Tests.Integration.AWS;

using System.Threading.Tasks;
using Amazon.SimpleNotificationService;

internal sealed class DisposableSubscription : IAsyncDisposable
{
    private readonly AmazonSimpleNotificationServiceClient _client;
    private readonly string _topicArn;
    public string SubscriptionArn { get; }

    public DisposableSubscription(AmazonSimpleNotificationServiceClient client, string topicArn, string subscriptionArn)
    {
        _client = client;
        _topicArn = topicArn;
        SubscriptionArn = subscriptionArn;
    }

    public async ValueTask DisposeAsync()
    {
        _ = await _client.DeleteTopicAsync(_topicArn);

        _client?.Dispose();
    }
}
