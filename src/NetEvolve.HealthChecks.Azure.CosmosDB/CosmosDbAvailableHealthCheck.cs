namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(CosmosDbAvailableOptions))]
internal sealed partial class CosmosDbAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CosmosDbAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var cosmosClient = clientCreation.GetCosmosClient(name, options, _serviceProvider);

        var readTask = ReadAccountPropertiesAsync(cosmosClient, options, cancellationToken);

        var (isValid, result) = await readTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "CosmosDB is not available.");
        }

        return HealthCheckState(isValid, name);
    }

    private static async Task<bool> ReadAccountPropertiesAsync(
        Microsoft.Azure.Cosmos.CosmosClient cosmosClient,
        CosmosDbAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        if (!string.IsNullOrWhiteSpace(options.DatabaseId))
        {
            var database = cosmosClient.GetDatabase(options.DatabaseId);
            _ = await database.ReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        else
        {
            _ = await cosmosClient.ReadAccountAsync().ConfigureAwait(false);
        }

        return true;
    }
}
