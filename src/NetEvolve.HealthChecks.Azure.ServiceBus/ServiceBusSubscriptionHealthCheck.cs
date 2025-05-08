namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ServiceBusSubscriptionHealthCheck : ConfigurableHealthCheckBase<ServiceBusSubscriptionOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceBusSubscriptionHealthCheck(
        IOptionsMonitor<ServiceBusSubscriptionOptions> optionsMonitor,
        IServiceProvider serviceProvider
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
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
        var client = ClientCreation.GetAdministrationClient(name, options, _serviceProvider);

        var (isValid, subscription) = await client
            .GetSubscriptionRuntimePropertiesAsync(options.TopicName, options.SubscriptionName, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && subscription is not null, name);
    }

    private async ValueTask<HealthCheckResult> ExecutePeekHealthCheckAsync(
        string name,
        ServiceBusSubscriptionOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = ClientCreation.GetClient(name, options, _serviceProvider);

        var receiver = client.CreateReceiver(options.TopicName, options.SubscriptionName);

        var (isValid, _) = await receiver
            .ReceiveMessageAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
