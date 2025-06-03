namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class BlobContainerAvailableHealthCheck : ConfigurableHealthCheckBase<BlobContainerAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public BlobContainerAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<BlobContainerAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
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

        var container = blobClient.GetBlobContainerClient(options.ContainerName);

        var containerExists = await container.ExistsAsync(cancellationToken).ConfigureAwait(false);
        if (!containerExists)
        {
            return HealthCheckResult.Unhealthy($"{name}: Container `{options.ContainerName}` does not exist.");
        }

        (var containerInTime, _) = await container
            .GetPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result && containerInTime, name);
    }
}
