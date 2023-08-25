﻿namespace NetEvolve.HealthChecks;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ApplicationSelfCheck : HealthCheckBase
{
    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthCheckContext context,
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
