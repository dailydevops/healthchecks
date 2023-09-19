#if USE_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;
using System;
using System.Threading;
using System.Threading.Tasks;

internal abstract class HealthCheckBase : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        Argument.ThrowIfNull(context);

        var name = context.Registration.Name;
        var failureStatus = context.Registration.FailureStatus;

        if (cancellationToken.IsCancellationRequested)
        {
            return new HealthCheckResult(
                failureStatus,
                description: $"{name}: Cancellation requested."
                );
        }

        try
        {
            return await ExecuteHealthCheckAsync(name, failureStatus, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(failureStatus, description: $"{name}: Unexpected error.", exception: ex);
        }
    }

    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    );
}
#endif
