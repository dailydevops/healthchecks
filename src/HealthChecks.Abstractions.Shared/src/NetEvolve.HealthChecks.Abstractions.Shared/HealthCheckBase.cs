#if USE_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

internal abstract class HealthCheckBase : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var configurationName = context.Registration.Name;
        var result = await InternalAsync(configurationName, context, cancellationToken).ConfigureAwait(false);

        return result;

        async ValueTask<HealthCheckResult> InternalAsync(string configurationName, HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                return await ExecuteHealthCheckAsync(configurationName, context, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }

    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthCheckContext context,
        CancellationToken cancellationToken
    );
}
#endif
