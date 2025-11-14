namespace NetEvolve.HealthChecks.Cassandra;

using System.Threading.Tasks;
using global::Cassandra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(CassandraOptions))]
internal sealed partial class CassandraHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        CassandraOptions options,
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
        using var session = await cluster.ConnectAsync().ConfigureAwait(false);

        var result = await session
            .ExecuteAsync(new SimpleStatement("SELECT release_version FROM system.local"))
            .ConfigureAwait(false);

        return result is not null && result.Any();
    }
}
