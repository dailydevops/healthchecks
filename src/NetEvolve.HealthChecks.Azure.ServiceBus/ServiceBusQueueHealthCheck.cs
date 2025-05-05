namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ServiceBusQueueHealthCheck : ConfigurableHealthCheckBase<ServiceBusQueueOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceBusQueueHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<ServiceBusQueueOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
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
        var client = ClientCreation.GetAdministrationClient(name, options, _serviceProvider);

        var (isValid, queue) = await client
            .GetQueueAsync(options.QueueName, cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && queue is not null, name);
    }

    private async ValueTask<HealthCheckResult> ExecutePeekHealthCheckAsync(
        string name,
        ServiceBusQueueOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = ClientCreation.GetClient(name, options, _serviceProvider);

        var receiver = client.CreateReceiver(options.QueueName);

        var (isValid, _) = await receiver
            .ReceiveMessageAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
