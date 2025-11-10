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
        HealthStatus failureStatus,
        JanusGraphOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IGremlinClient>()
            : _serviceProvider.GetRequiredKeyedService<IGremlinClient>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "The command did not return a successful result.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(IGremlinClient client, CancellationToken cancellationToken)
    {
        _ = await client
            .SubmitAsync<long>("g.V().limit(1).count()", cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return true;
    }
}
