namespace NetEvolve.HealthChecks;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

internal sealed class ApplicationHealthyCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        var failureStatus = context.Registration.FailureStatus;

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("ApplicationHealthy: Unhealthy"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("ApplicationHealthy: Healthy"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                new HealthCheckResult(
                    failureStatus,
                    description: "ApplicationHealthy: Unexpected error.",
                    exception: ex
                )
            );
        }
    }
}
