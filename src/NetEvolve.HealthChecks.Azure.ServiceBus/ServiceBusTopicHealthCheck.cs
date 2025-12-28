namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ServiceBusTopicOptions))]
internal sealed partial class ServiceBusTopicHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        ServiceBusTopicOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<ServiceBusClientFactory>();
        var client = clientFactory.GetAdministrationClient(name, options, _serviceProvider);

        var (isTimelyResponse, _) = await client
            .GetTopicRuntimePropertiesAsync(options.TopicName, cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
