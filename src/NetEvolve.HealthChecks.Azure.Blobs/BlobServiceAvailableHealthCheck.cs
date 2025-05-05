namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class BlobServiceAvailableHealthCheck : ConfigurableHealthCheckBase<BlobServiceAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public BlobServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<BlobServiceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        BlobServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var blobClient = ClientCreation.GetBlobServiceClient(name, options, _serviceProvider);

        var blobTask = blobClient
            .GetBlobContainersAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync();

        var (isValid, result) = await blobTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result, name);
    }
}
