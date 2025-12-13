namespace NetEvolve.HealthChecks.Meilisearch;

using System.Threading.Tasks;
using Meilisearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(MeilisearchOptions))]
internal sealed partial class MeilisearchHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        MeilisearchOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientProvider = _serviceProvider.GetRequiredService<MeilisearchClientProvider>();
        var client = clientProvider.GetClient(name, options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, isResultValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse && isResultValid, name);
    }

    internal static async Task<bool> DefaultCommandAsync(
        global::Meilisearch.MeilisearchClient client,
        CancellationToken cancellationToken
    )
    {
        var isHealthy = await client.IsHealthyAsync(cancellationToken).ConfigureAwait(false);
        return isHealthy;
    }
}
