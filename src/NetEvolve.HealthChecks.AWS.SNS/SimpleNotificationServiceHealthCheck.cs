namespace NetEvolve.HealthChecks.AWS.SNS;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SimpleNotificationServiceHealthCheck
    : ConfigurableHealthCheckBase<SimpleNotificationServiceOptions>
{
    public SimpleNotificationServiceHealthCheck(IOptionsMonitor<SimpleNotificationServiceOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SimpleNotificationServiceOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);

        var (isTimelyResponse, topic) = await client
            .FindTopicAsync(options.TopicName)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(topic?.TopicArn))
        {
            return HealthCheckUnhealthy(failureStatus, name, $"Topic `{options.TopicName}` not found.");
        }

        var (isTimelyResponse2, subscriptions) = await GetSubscriptionsAsync(client, topic, options, cancellationToken)
            .ConfigureAwait(false);
        var found = subscriptions.Any(x => x.EndsWith(options.Subscription!, StringComparison.Ordinal));

        if (!found)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"Subscription `{options.Subscription}` not found.");
        }

        return HealthCheckState(isTimelyResponse && isTimelyResponse2, name);
    }

    private static async Task<(bool, List<string>)> GetSubscriptionsAsync(
        AmazonSimpleNotificationServiceClient client,
        Topic topic,
        SimpleNotificationServiceOptions options,
        CancellationToken cancellationToken
    )
    {
        var (isValid, response) = await client
            .ListSubscriptionsByTopicAsync(topic.TopicArn, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new InvalidOperationException($"Invalid response from AWS: {response.HttpStatusCode}");
        }

        var subscriptions = response.Subscriptions.ConvertAll(x => x.SubscriptionArn);

        while (!string.IsNullOrEmpty(response.NextToken))
        {
            (isValid, response) = await client
                .ListSubscriptionsByTopicAsync(topic.TopicArn, response.NextToken, cancellationToken)
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"Invalid response from AWS: {response.HttpStatusCode}");
            }

            subscriptions.AddRange(response.Subscriptions.Select(x => x.SubscriptionArn));
        }

        return (isValid, subscriptions);
    }

    private static AmazonSimpleNotificationServiceClient CreateClient(SimpleNotificationServiceOptions options)
    {
        var config = new AmazonSimpleNotificationServiceConfig { ServiceURL = options.ServiceUrl };

        var credentials = options.GetCredentials();

        return (credentials is not null, options.RegionEndpoint is not null) switch
        {
            (true, true) => new AmazonSimpleNotificationServiceClient(credentials, options.RegionEndpoint),
            (true, false) => new AmazonSimpleNotificationServiceClient(credentials, config),
            (false, true) => new AmazonSimpleNotificationServiceClient(options.RegionEndpoint),
            _ => new AmazonSimpleNotificationServiceClient(config),
        };
    }
}
