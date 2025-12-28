namespace NetEvolve.HealthChecks.Azure.Blobs;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(BlobContainerAvailableOptions))]
internal sealed partial class BlobContainerAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        BlobContainerAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var blobClient = clientCreation.GetBlobServiceClient(name, options, _serviceProvider);

        var blobTask = blobClient
            .GetBlobContainersAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync();

        var (isValid, result) = await blobTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Failed to list blob containers.");
        }

        var container = blobClient.GetBlobContainerClient(options.ContainerName);

        var containerExists = await container.ExistsAsync(cancellationToken).ConfigureAwait(false);
        if (!containerExists)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"Container `{options.ContainerName}` does not exist.");
        }

        (var containerInTime, _) = await container
            .GetPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && containerInTime, name);
    }
}
