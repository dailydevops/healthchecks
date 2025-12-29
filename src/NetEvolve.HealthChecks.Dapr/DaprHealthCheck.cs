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
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
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

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"{name} reported unhealthy status from Dapr sidecar.");
        }

        return HealthCheckState(isHealthy && result, name);
    }
}
