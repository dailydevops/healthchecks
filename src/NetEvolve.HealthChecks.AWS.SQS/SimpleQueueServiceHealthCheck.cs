namespace NetEvolve.HealthChecks.AWS.SQS;

using System.Net;
using Amazon.SQS;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(SimpleQueueServiceOptions))]
internal sealed partial class SimpleQueueServiceHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SimpleQueueServiceOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);
        var (isTimelyResponse, response) = await client
            .GetQueueUrlAsync(options.QueueName, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Unexpected HTTP status code: {response.HttpStatusCode}."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private static AmazonSQSClient CreateClient(SimpleQueueServiceOptions options)
    {
        var config = new AmazonSQSConfig { ServiceURL = options.ServiceUrl };

        var credentials = options.GetCredentials();

        return (credentials is not null, options.RegionEndpoint is not null) switch
        {
            (true, true) => new AmazonSQSClient(credentials, options.RegionEndpoint),
            (true, false) => new AmazonSQSClient(credentials, config),
            (false, true) => new AmazonSQSClient(options.RegionEndpoint),
            _ => new AmazonSQSClient(config),
        };
    }
}
