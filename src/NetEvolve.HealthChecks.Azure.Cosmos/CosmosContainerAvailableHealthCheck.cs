namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class CosmosContainerAvailableHealthCheck : ConfigurableHealthCheckBase<CosmosContainerAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public CosmosContainerAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<CosmosContainerAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CosmosContainerAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var cosmosClient = ClientCreation.GetCosmosClient(name, options, _serviceProvider);

        // Try to read container properties
        var (isValid, _) = await cosmosClient
            .GetContainer(options.DatabaseId, options.ContainerId)
            .ReadContainerAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
