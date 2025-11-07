namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ServiceBusSubscriptionOptions))]
internal sealed partial class ServiceBusSubscriptionHealthCheck
{
    private ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        ServiceBusSubscriptionOptions options,
        CancellationToken cancellationToken
    ) =>
        options.EnablePeekMode
            ? ExecutePeekHealthCheckAsync(name, options, cancellationToken)
            : ExecuteHealthCheckAsync(name, options, cancellationToken);

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        ServiceBusSubscriptionOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<ServiceBusClientFactory>();
        var client = clientFactory.GetAdministrationClient(name, options, _serviceProvider);

        var (isValid, _) = await client
            .GetSubscriptionRuntimePropertiesAsync(options.TopicName, options.SubscriptionName, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }

    private async ValueTask<HealthCheckResult> ExecutePeekHealthCheckAsync(
        string name,
        ServiceBusSubscriptionOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<ServiceBusClientFactory>();
        var client = clientFactory.GetClient(name, options, _serviceProvider);

        var receiver = client.CreateReceiver(options.TopicName, options.SubscriptionName);

        var (isValid, _) = await receiver
            .PeekMessageAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
