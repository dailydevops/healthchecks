namespace NetEvolve.HealthChecks.AWS.SNS;

using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SimpleNotificationServiceHealthCheck
    : ConfigurableHealthCheckBase<SimpleNotificationServiceOptions>
{
    public SimpleNotificationServiceHealthCheck(
        IOptionsMonitor<SimpleNotificationServiceOptions> optionsMonitor
    )
        : base(optionsMonitor) { }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SimpleNotificationServiceOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);

        var (isValid, topic) = await client
            .GetSubscriptionAttributesAsync(
                new GetSubscriptionAttributesRequest { SubscriptionArn = options.TopicName },
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && topic is not null, name);
    }

    private static AmazonSimpleNotificationServiceClient CreateClient(
        SimpleNotificationServiceOptions options
    )
    {
        var hasCredentials = options.GetCredentials() is not null;
        var hasEndpoint = options.GetRegionEndpoint() is not null;

        var config = new AmazonSimpleNotificationServiceConfig
        {
            ServiceURL = options.ServiceUrl,
            RegionEndpoint = RegionEndpoint.USEast1,
        };

        return (hasCredentials, hasEndpoint) switch
        {
            (true, true) => new AmazonSimpleNotificationServiceClient(
                options.GetCredentials(),
                options.GetRegionEndpoint()
            ),
            (true, false) => new AmazonSimpleNotificationServiceClient(
                options.GetCredentials(),
                config
            ),
            (false, true) => new AmazonSimpleNotificationServiceClient(options.GetRegionEndpoint()),
            _ => throw new InvalidOperationException("Invalid ClientCreationMode."),
        };
    }
}
