namespace NetEvolve.HealthChecks.Couchbase;

using System.Threading.Tasks;
using global::Couchbase;
using global::Couchbase.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(CouchbaseOptions))]
internal sealed partial class CouchbaseHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CouchbaseOptions options,
        CancellationToken cancellationToken
    )
    {
        var cluster = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<ICluster>()
            : _serviceProvider.GetRequiredKeyedService<ICluster>(options.KeyedService);

        var (isTimelyResponse, result) = await options
            .CommandAsync(cluster, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Couchbase health check command returned false.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(ICluster cluster, CancellationToken cancellationToken)
    {
        var result = await cluster.PingAsync(_options).ConfigureAwait(false);

        return result is not null
            && result.Services.Values.SelectMany(service => service).All(endpoint => endpoint.State == ServiceState.Ok);
    }

    private static readonly PingOptions _options = new PingOptions().ServiceTypes(
        ServiceType.KeyValue,
        ServiceType.Query
    );
}
