namespace NetEvolve.HealthChecks.Azure.EventHubs;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class EventHubHealthCheck : ConfigurableHealthCheckBase<EventHubOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public EventHubHealthCheck(IOptionsMonitor<EventHubOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        EventHubOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<EventHubsClientFactory>();
        var client = clientFactory.GetClient(name, options, _serviceProvider);

        var (isValid, _) = await client
            .GetEventHubPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
