namespace NetEvolve.HealthChecks.GCP.PubSub;

using System.Threading;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(PubSubOptions))]
internal sealed partial class PubSubHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        PubSubOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<PublisherServiceApiClient>()
            : _serviceProvider.GetRequiredKeyedService<PublisherServiceApiClient>(options.KeyedService);

        // Use a simple operation to check if the Pub/Sub service is accessible
        // List topics is a lightweight operation that verifies the client can connect to the service
        var projectName = new ProjectName(options.ProjectName);

        // Attempt a lightweight operation with timeout
        var (isTimelyResponse, result) = await Task.Run(
                async () =>
                {
                    var result = client.ListTopicsAsync(projectName);
                    var topics = await result.ReadPageAsync(1).ConfigureAwait(false);
                    return topics?.GetEnumerator().MoveNext() ?? false;
                },
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Pub/Sub service is not responding as expected.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
