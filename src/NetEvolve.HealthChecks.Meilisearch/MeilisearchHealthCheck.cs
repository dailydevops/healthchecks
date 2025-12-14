namespace NetEvolve.HealthChecks.Meilisearch;

using System.Threading.Tasks;
using global::Meilisearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(MeilisearchOptions))]
internal sealed partial class MeilisearchHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
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

        if (!isResultValid)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Received an invalid response from Meilisearch.");
        }

        return HealthCheckState(isTimelyResponse && isResultValid, name);
    }

    internal static async Task<bool> DefaultCommandAsync(
        MeilisearchClient client,
        CancellationToken cancellationToken
    ) => await client.IsHealthyAsync(cancellationToken).ConfigureAwait(false);
}
