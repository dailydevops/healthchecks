namespace NetEvolve.HealthChecks.Azure.IotHub;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class IotHubAvailabilityHealthCheck : ConfigurableHealthCheckBase<IotHubAvailabilityOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public IotHubAvailabilityHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<IotHubAvailabilityOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        IotHubAvailabilityOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientFactory = _serviceProvider.GetRequiredService<IotHubClientFactory>();
        var serviceClient = clientFactory.GetServiceClient(name, options, _serviceProvider);

        var (isValid, _) = await serviceClient
            .GetServiceStatisticsAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}