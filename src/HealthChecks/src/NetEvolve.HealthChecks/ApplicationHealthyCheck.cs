﻿namespace NetEvolve.HealthChecks;

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
            return new ValueTask<HealthCheckResult>(
                HealthCheckResult.Unhealthy($"{name}: Unhealthy")
            );
        }

        return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy($"{name}: Healthy"));
    }
}
