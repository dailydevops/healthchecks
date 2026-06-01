namespace NetEvolve.HealthChecks.Azure.EventHubs;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(EventHubsOptions), includeWin32Handling: true)]
internal sealed partial class EventHubsHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        EventHubsOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<EventHubsClientFactory>();
        var client = clientFactory.GetClient(name, options, _serviceProvider);

        var (isTimelyResponse, hub) = await client
            .GetEventHubPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (hub is null)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Event hub not found.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
