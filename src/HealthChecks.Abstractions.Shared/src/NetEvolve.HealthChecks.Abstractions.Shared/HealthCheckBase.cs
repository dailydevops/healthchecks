#if USE_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

internal abstract class HealthCheckBase : IHealthCheck
{
    public HealthCheckBase() { }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var result = await InternalAsync(context, cancellationToken).ConfigureAwait(false);

        var data = (Dictionary<string, object>)result.Data;
        _ = data.TryAdd("Type", GetType().Name);
        _ = data.TryAdd("ConfigurationName", context.Registration.Name);


        return result;

        async ValueTask<HealthCheckResult> InternalAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                return await ExecuteHealthCheckAsync(context, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }

    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken
    );
}
#endif
