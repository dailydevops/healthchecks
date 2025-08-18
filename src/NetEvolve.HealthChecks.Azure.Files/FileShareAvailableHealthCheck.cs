namespace NetEvolve.HealthChecks.Azure.Files;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class FileShareAvailableHealthCheck : ConfigurableHealthCheckBase<FileShareAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public FileShareAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<FileShareAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        FileShareAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var shareClient = clientCreation.GetShareServiceClient(name, options, _serviceProvider);

        var shareTask = shareClient
            .GetSharesAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync();

        var (isValid, result) = await shareTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        var share = shareClient.GetShareClient(options.ShareName);

        var shareExists = await share.ExistsAsync(cancellationToken).ConfigureAwait(false);
        if (!shareExists)
        {
            return HealthCheckResult.Unhealthy($"{name}: File share `{options.ShareName}` does not exist.");
        }

        (var shareInTime, _) = await share
            .GetPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result && shareInTime, name);
    }
}