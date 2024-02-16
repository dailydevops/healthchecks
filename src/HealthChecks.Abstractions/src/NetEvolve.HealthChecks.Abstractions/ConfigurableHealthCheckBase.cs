namespace NetEvolve.HealthChecks.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

/// <summary>
/// Configurable implementation of <see cref="IHealthCheck"/>.
/// </summary>
/// <typeparam name="TConfiguration">Type of Configuration</typeparam>
public abstract class ConfigurableHealthCheckBase<TConfiguration> : IHealthCheck
    where TConfiguration : class
{
    private readonly IOptionsMonitor<TConfiguration> _optionsMonitor;

    /// <inheritdoc/>
    protected ConfigurableHealthCheckBase(IOptionsMonitor<TConfiguration> optionsMonitor) =>
        _optionsMonitor = optionsMonitor;

    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        var configurationName = context.Registration.Name;
        var failureStatus = context.Registration.FailureStatus;

        if (cancellationToken.IsCancellationRequested)
        {
            return new HealthCheckResult(
                failureStatus,
                description: $"{configurationName}: Cancellation requested."
            );
        }

        var result = await InternalAsync(configurationName, failureStatus, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private async Task<HealthCheckResult> InternalAsync(
        string configurationName,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var options = _optionsMonitor.Get(configurationName);
            if (options is null)
            {
                return new HealthCheckResult(
                    HealthStatus.Unhealthy,
                    description: $"{configurationName}: Missing configuration."
                );
            }

            return await ExecuteHealthCheckAsync(
                    configurationName,
                    failureStatus,
                    options,
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

    /// <summary>
    /// Abstract method that executes the necessary business logic of each implementation.
    /// </summary>
    /// <param name="name">Configuration Name</param>
    /// <param name="failureStatus">Configured <see cref="HealthStatus"/> in case of failure.</param>
    /// <param name="options">Configuration object of <typeparamref name="TConfiguration"/>.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        TConfiguration options,
        CancellationToken cancellationToken
    );
}
