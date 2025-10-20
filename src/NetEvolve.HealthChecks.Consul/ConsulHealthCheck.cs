namespace NetEvolve.HealthChecks.Consul;

using System.Threading;
using System.Threading.Tasks;
using global::Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ConsulHealthCheck : ConfigurableHealthCheckBase<ConsulOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public ConsulHealthCheck(IOptionsMonitor<ConsulOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus failureStatus,
        ConsulOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IConsulClient>()
            : _serviceProvider.GetRequiredKeyedService<IConsulClient>(options.KeyedService);

        var (isValid, response) = await client
            .Status.Leader(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(response))
        {
            return HealthCheckUnhealthy(failureStatus, name);
        }

        return HealthCheckState(isValid, name);
    }
}
