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
        HealthStatus failureStatus,
        ServiceBusSubscriptionOptions options,
        CancellationToken cancellationToken
    ) =>
        options.EnablePeekMode
            ? ExecutePeekHealthCheckAsync(name, options, cancellationToken)
            : ExecuteGetHealthCheckAsync(name, failureStatus, options, cancellationToken);

    private async ValueTask<HealthCheckResult> ExecuteGetHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ServiceBusSubscriptionOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<ServiceBusClientFactory>();
        var client = clientFactory.GetAdministrationClient(name, options, _serviceProvider);

        var (isValid, subscription) = await client
            .GetSubscriptionRuntimePropertiesAsync(options.TopicName, options.SubscriptionName, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Subscription not found.");
        }

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
