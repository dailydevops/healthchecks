namespace NetEvolve.HealthChecks.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Non-configurable standard implementation of <see cref="IHealthCheck"/>.
/// </summary>
public abstract class HealthCheckBase : IHealthCheck
{
    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        var name = context.Registration.Name;
        var failureStatus = context.Registration.FailureStatus;

        try
        {
            return await ExecuteHealthCheckAsync(name, failureStatus, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(failureStatus, description: $"{name}: Unexpected error.", exception: ex);
        }
    }

    /// <summary>
    /// Abstract method that executes the necessary business logic of each implementation.
    /// </summary>
    /// <param name="name">Configuration Name</param>
    /// <param name="failureStatus">Configured <see cref="HealthStatus"/> in case of failure.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    );
}
