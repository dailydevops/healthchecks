namespace NetEvolve.HealthChecks.Dapr;

using System.Threading;
using System.Threading.Tasks;
using global::Dapr.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(DaprOptions))]
internal sealed partial class DaprHealthCheck
{
    /// <summary>
    /// Performs the Dapr health probe and produces a HealthCheckResult for the specified check name.
    /// </summary>
    /// <param name="name">The health check name used when constructing the result.</param>
    /// <param name="options">DaprOptions that configure the probe (e.g., Timeout and KeyedService).</param>
    /// <returns>`HealthCheckResult` representing a healthy state only if the Dapr client reports healthy and the probe result is true; otherwise an unhealthy result for the given name.</returns>
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        DaprOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<DaprClient>()
            : _serviceProvider.GetRequiredKeyedService<DaprClient>(options.KeyedService);

        var (isHealthy, result) = await client
            .CheckHealthAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isHealthy && result, name);
    }
}