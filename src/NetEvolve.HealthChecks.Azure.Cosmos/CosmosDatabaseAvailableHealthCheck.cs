namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class CosmosDatabaseAvailableHealthCheck : ConfigurableHealthCheckBase<CosmosDatabaseAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public CosmosDatabaseAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<CosmosDatabaseAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CosmosDatabaseAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var cosmosClient = ClientCreation.GetCosmosClient(name, options, _serviceProvider);

        // Try to read database properties
        var (isValid, _) = await cosmosClient
            .GetDatabase(options.DatabaseId)
            .ReadAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
