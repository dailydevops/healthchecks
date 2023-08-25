#if USE_CONFIGURABLE_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

internal abstract class ConfigurableHealthCheckBase<TConfiguration> : IHealthCheck
    where TConfiguration : class
{
    private readonly IOptionsMonitor<TConfiguration> _optionsMonitor;

    public ConfigurableHealthCheckBase(IOptionsMonitor<TConfiguration> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var configurationName = context.Registration.Name;
        var result = await InternalAsync(context, configurationName, cancellationToken).ConfigureAwait(false);

        return result;

        async Task<HealthCheckResult> InternalAsync(HealthCheckContext context, string configurationName, CancellationToken cancellationToken)
        {
            try
            {
                var options = _optionsMonitor.Get(configurationName);
                if (options is null)
                {
                    return new HealthCheckResult(
                        HealthStatus.Unhealthy,
                        description: $"{configurationName}: Missing configuration"
                    );
                }

                return await ExecuteHealthCheckAsync(
                        configurationName,
                        options,
                        context,
                        cancellationToken
                    )
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }

    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        TConfiguration options,
        HealthCheckContext context,
        CancellationToken cancellationToken
    );
}
#endif
