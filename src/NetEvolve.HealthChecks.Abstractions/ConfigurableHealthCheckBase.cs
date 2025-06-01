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
    where TConfiguration : class, IEquatable<TConfiguration>, new()
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

        var name = context.Registration.Name;
        var failureStatus = context.Registration.FailureStatus;

        if (cancellationToken.IsCancellationRequested)
        {
            return new HealthCheckResult(failureStatus, description: $"{name}: Cancellation requested.");
        }

        return await InternalAsync(name, failureStatus, cancellationToken).ConfigureAwait(false);
    }

    private async Task<HealthCheckResult> InternalAsync(
        string name,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var options = _optionsMonitor.Get(name);
            if (options?.Equals(new TConfiguration()) != false)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, description: $"{name}: Missing configuration.");
            }

            return await ExecuteHealthCheckAsync(name, failureStatus, options, cancellationToken).ConfigureAwait(false);
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
    /// <param name="options">Configuration object of <typeparamref name="TConfiguration"/>.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    protected abstract ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        TConfiguration options,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Returns a <see cref="HealthCheckResult"/> based on the value of the <paramref name="condition"/>.
    /// </summary>
    /// <param name="condition">
    /// When <see langword="true"/>, returns a <see cref="HealthCheckResult"/> with <see cref="HealthStatus.Healthy"/>. Otherwise, returns <see cref="HealthStatus.Degraded"/>.
    /// </param>
    /// <param name="name">
    /// Name of the Health Check Configuration.
    /// </param>
    /// <returns>
    /// A <see cref="HealthCheckResult"/> with <see cref="HealthStatus.Healthy"/> or <see cref="HealthStatus.Degraded"/>.
    /// </returns>
    protected static HealthCheckResult HealthCheckState(bool condition, string name) =>
        condition ? HealthCheckResult.Healthy($"{name}: Healthy") : HealthCheckDegraded(name);

    /// <summary>
    /// Creates a <see cref="HealthCheckResult"/> with the specified failure status, indicating an unhealthy state.
    /// </summary>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> to use for the unhealthy result.</param>
    /// <param name="name">The name of the health check to include in the description.</param>
    /// <param name="message">Additional message to include in the description. Defaults to "Unhealthy".</param>
    /// <returns>
    /// A <see cref="HealthCheckResult"/> with the specified <paramref name="failureStatus"/> and a description
    /// formatted as "{name}: {message}".
    /// </returns>
    protected static HealthCheckResult HealthCheckUnhealthy(
        HealthStatus failureStatus,
        string name,
        string message = "Unhealthy"
    ) => new HealthCheckResult(failureStatus, $"{name}: {message}");

    /// <summary>
    /// Returns a <see cref="HealthCheckResult"/> with <see cref="HealthStatus.Degraded"/>.
    /// </summary>
    /// <param name="name">
    /// Name of the Health Check Configuration.
    /// </param>
    /// <returns>
    /// A <see cref="HealthCheckResult"/> with <see cref="HealthStatus.Degraded"/>.
    /// </returns>
    protected static HealthCheckResult HealthCheckDegraded(string name) =>
        HealthCheckResult.Degraded($"{name}: Degraded");
}
