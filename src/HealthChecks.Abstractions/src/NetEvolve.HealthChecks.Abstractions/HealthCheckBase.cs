namespace NetEvolve.HealthChecks.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;

public abstract class HealthCheckBase : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        Argument.ThrowIfNull(context);

        var configurationName = context.Registration.Name;
        var failureStatus = context.Registration.FailureStatus;
        var result = await InternalAsync(configurationName, failureStatus, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private async ValueTask<HealthCheckResult> InternalAsync(
        string configurationName,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await ExecuteHealthCheckAsync(
                    configurationName,
                    failureStatus,
                    cancellationToken
                )
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                failureStatus,
                description: $"{configurationName}: Unexpected error.",
                exception: ex
            );
        }
    }

    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    );
}
