namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class CosmosClientAvailableHealthCheck : ConfigurableHealthCheckBase<CosmosClientAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public CosmosClientAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<CosmosClientAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CosmosClientAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var cosmosClient = ClientCreation.GetCosmosClient(name, options, _serviceProvider);

        // Check if we can get database accounts list
        var (isValid, _) = await cosmosClient
            .ReadAccountAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
