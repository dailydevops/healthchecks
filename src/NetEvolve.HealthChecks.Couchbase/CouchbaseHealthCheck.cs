namespace NetEvolve.HealthChecks.Couchbase;

using System.Threading.Tasks;
using global::Couchbase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(CouchbaseOptions))]
internal sealed partial class CouchbaseHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        CouchbaseOptions options,
        CancellationToken cancellationToken
    )
    {
        var cluster = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<ICluster>()
            : _serviceProvider.GetRequiredKeyedService<ICluster>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(cluster, cancellationToken);

        var (isTimelyResponse, _) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(ICluster cluster, CancellationToken cancellationToken)
    {
        var result = await cluster.PingAsync().ConfigureAwait(false);

        if (result is null || result.Services.Count == 0)
        {
            throw new InvalidOperationException("Couchbase ping failed.");
        }

        return true;
    }
}
