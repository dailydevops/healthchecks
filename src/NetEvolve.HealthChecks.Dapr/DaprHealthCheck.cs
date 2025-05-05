namespace NetEvolve.HealthChecks.Dapr;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Dapr.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class DaprHealthCheck : ConfigurableHealthCheckBase<DaprOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public DaprHealthCheck(IServiceProvider serviceProvider, IOptionsMonitor<DaprOptions> optionsMonitor)
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        DaprOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = _serviceProvider.GetRequiredService<DaprClient>();

        var (isHealthy, result) = await client
            .CheckHealthAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isHealthy && result, name);
    }
}
