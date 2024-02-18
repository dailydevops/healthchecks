namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;

internal sealed class BlobServiceAvailableHealthCheck
    : BlobHealthCheckBase<BlobServiceAvailableOptions>
{
    public BlobServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<BlobServiceAvailableOptions> optionsMonitor
    )
        : base(serviceProvider, optionsMonitor) { }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        BlobServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var blobClient = GetBlobServiceClient(name, options, _serviceProvider);

        var blobTask = blobClient
            .GetBlobContainersAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync();

        var (isValid, result) = await blobTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return (isValid && result)
            ? HealthCheckResult.Healthy($"{name}: Healthy")
            : HealthCheckResult.Degraded($"{name}: Degraded");
    }
}
