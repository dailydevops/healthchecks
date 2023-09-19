namespace NetEvolve.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Abstractions;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ApplicationHealthyCheck : HealthCheckBase
{
    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromResult(HealthCheckResult.Unhealthy($"{name}: Unhealthy"));
        }

        return ValueTask.FromResult(HealthCheckResult.Healthy($"{name}: Healthy"));
    }
}
