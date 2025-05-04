namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ServiceBusTopicHealthCheck
    : ConfigurableHealthCheckBase<ServiceBusTopicOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceBusTopicHealthCheck(
        IOptionsMonitor<ServiceBusTopicOptions> optionsMonitor,
        IServiceProvider serviceProvider
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ServiceBusTopicOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = ClientCreation.GetAdministrationClient(name, options, _serviceProvider);

        var (isValid, _) = await client
            .GetTopicAsync(options.TopicName, cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
