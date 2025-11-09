namespace NetEvolve.HealthChecks.JanusGraph;

using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(JanusGraphOptions))]
internal sealed partial class JanusGraphHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        JanusGraphOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IGremlinClient>()
            : _serviceProvider.GetRequiredKeyedService<IGremlinClient>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, _) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<object> DefaultCommandAsync(
#pragma warning disable S1172 // Unused method parameters should be removed
        IGremlinClient client, CancellationToken cancellationToken
#pragma warning restore S1172 // Unused method parameters should be removed
    )
    {
#pragma warning disable CA2016 // Forward the CancellationToken parameter to methods
        var result = await client.SubmitAsync<int>("g.V().limit(1).count()").ConfigureAwait(false);
#pragma warning restore CA2016 // Forward the CancellationToken parameter to methods

        return result;
    }
}
