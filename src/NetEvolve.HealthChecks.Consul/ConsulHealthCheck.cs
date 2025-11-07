namespace NetEvolve.HealthChecks.Consul;

using System.Threading;
using System.Threading.Tasks;
using global::Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ConsulOptions))]
internal sealed partial class ConsulHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus failureStatus,
        ConsulOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IConsulClient>()
            : _serviceProvider.GetRequiredKeyedService<IConsulClient>(options.KeyedService);

        var (isTimelyResponse, response) = await client
            .Status.Leader(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(response))
        {
            return HealthCheckUnhealthy(failureStatus, name);
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
