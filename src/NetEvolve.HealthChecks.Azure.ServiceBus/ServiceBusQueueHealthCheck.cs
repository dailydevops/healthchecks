namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ServiceBusQueueOptions))]
internal sealed partial class ServiceBusQueueHealthCheck
{
    private ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ServiceBusQueueOptions options,
        CancellationToken cancellationToken
    ) =>
        options.EnablePeekMode
            ? ExecutePeekHealthCheckAsync(name, options, cancellationToken)
            : ExecuteGetHealthCheckAsync(name, failureStatus, options, cancellationToken);

    private async ValueTask<HealthCheckResult> ExecuteGetHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ServiceBusQueueOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<ServiceBusClientFactory>();
        var client = clientFactory.GetAdministrationClient(name, options, _serviceProvider);

        var (isTimelyResponse, queue) = await client
            .GetQueueRuntimePropertiesAsync(options.QueueName, cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (queue is null)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Queue not found.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private async ValueTask<HealthCheckResult> ExecutePeekHealthCheckAsync(
        string name,
        ServiceBusQueueOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<ServiceBusClientFactory>();
        var client = clientFactory.GetClient(name, options, _serviceProvider);

        var receiver = client.CreateReceiver(options.QueueName);

        var (isTimelyResponse, _) = await receiver
            .PeekMessageAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
