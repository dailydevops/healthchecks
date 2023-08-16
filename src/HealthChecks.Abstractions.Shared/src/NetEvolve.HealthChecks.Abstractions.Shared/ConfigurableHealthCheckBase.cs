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
    private readonly string _configurationNamePrefix;

    public ConfigurableHealthCheckBase(IOptionsMonitor<TConfiguration> optionsMonitor, string configurationNamePrefix = "")
    {
        _optionsMonitor = optionsMonitor;
        _configurationNamePrefix = configurationNamePrefix;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var configurationName = context.Registration.Name;
        if (!string.IsNullOrWhiteSpace(_configurationNamePrefix))
        {
            configurationName = $"{_configurationNamePrefix}{configurationName}";
        }
        var result = await InternalAsync(context, configurationName, cancellationToken).ConfigureAwait(false);

        var data = (Dictionary<string, object>)result.Data;
        _ = data.TryAdd("Type", GetType().Name);
        _ = data.TryAdd("ConfigurationName", configurationName);

        return result;

        async Task<HealthCheckResult> InternalAsync(HealthCheckContext context, string configurationName, CancellationToken cancellationToken)
        {
            try
            {
                var options = _optionsMonitor.Get(configurationName);
                if (options is null)
                {
                    return new HealthCheckResult(
                        context.Registration.FailureStatus,
                        description: $"Missing configuration for '{configurationName}'"
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
