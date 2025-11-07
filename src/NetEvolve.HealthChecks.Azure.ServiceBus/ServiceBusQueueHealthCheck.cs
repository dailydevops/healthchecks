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
            : ExecuteHealthCheckAsync(name, options, cancellationToken);

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        ServiceBusQueueOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<ServiceBusClientFactory>();
        var client = clientFactory.GetAdministrationClient(name, options, _serviceProvider);

        var (isValid, _) = await client
            .GetQueueRuntimePropertiesAsync(options.QueueName, cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
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

        var (isValid, _) = await receiver
            .PeekMessageAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
