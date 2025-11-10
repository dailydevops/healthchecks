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
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        PubSubOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<PublisherServiceApiClient>()
            : _serviceProvider.GetRequiredKeyedService<PublisherServiceApiClient>(options.KeyedService);

        // Use a simple operation to check if the Pub/Sub service is accessible
        // List topics is a lightweight operation that verifies the client can connect to the service
        var projectName = new ProjectName("_");

        // Attempt a lightweight operation with timeout
        var listTopicsTask = Task.Run(
            async () =>
            {
                var result = client.ListTopicsAsync(projectName);
                return await result.ReadPageAsync(1).ConfigureAwait(false);
            },
            cancellationToken
        );

        var (isTimelyResponse, _) = await listTopicsTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
