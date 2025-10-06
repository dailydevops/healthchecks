namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class CosmosDbHealthCheck : ConfigurableHealthCheckBase<CosmosDbOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public CosmosDbHealthCheck(IServiceProvider serviceProvider, IOptionsMonitor<CosmosDbOptions> optionsMonitor)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CosmosDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var cosmosClient = clientCreation.GetCosmosClient(name, options, _serviceProvider);

        // Check if the CosmosDB service is available by reading the account properties
        var (serviceAvailable, _) = await cosmosClient
            .ReadAccountAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!serviceAvailable)
        {
            return HealthCheckState(false, name);
        }

        // If database name is specified, check database availability
        if (!string.IsNullOrWhiteSpace(options.DatabaseName))
        {
            var database = cosmosClient.GetDatabase(options.DatabaseName);
            var (databaseAvailable, _) = await database
                .ReadAsync(cancellationToken: cancellationToken)
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            if (!databaseAvailable)
            {
                return HealthCheckResult.Unhealthy($"{name}: Database `{options.DatabaseName}` is not available.");
            }

            // If container name is also specified, check container availability
            if (!string.IsNullOrWhiteSpace(options.ContainerName))
            {
                var container = database.GetContainer(options.ContainerName);
                var (containerAvailable, _) = await container
                    .ReadContainerAsync(cancellationToken: cancellationToken)
                    .WithTimeoutAsync(options.Timeout, cancellationToken)
                    .ConfigureAwait(false);

                if (!containerAvailable)
                {
                    return HealthCheckResult.Unhealthy(
                        $"{name}: Container `{options.ContainerName}` is not available."
                    );
                }
            }
        }

        return HealthCheckState(true, name);
    }
}
