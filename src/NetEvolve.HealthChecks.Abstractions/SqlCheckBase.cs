namespace NetEvolve.HealthChecks.Abstractions;

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;

/// <summary>
/// Configurable implementation of <see cref="IHealthCheck"/> with focus on <see cref="DbConnection"/> based implementations.
/// </summary>
/// <typeparam name="TConfiguration">Type of Configuration</typeparam>
public abstract class SqlCheckBase<TConfiguration> : IHealthCheck
    where TConfiguration : class, ISqlCheckOptions
{
    private readonly IOptionsMonitor<TConfiguration> _optionsMonitor;

    /// <inheritdoc/>
    protected SqlCheckBase(IOptionsMonitor<TConfiguration> optionsMonitor) =>
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

        return await InternalAsync(configurationName, failureStatus, cancellationToken)
            .ConfigureAwait(false);
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

            return await ExecuteHealthCheckAsync(configurationName, options, cancellationToken)
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

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "As designed."
    )]
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        TConfiguration options,
        CancellationToken cancellationToken
    )
    {
        using (var connection = CreateConnection(options.ConnectionString))
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = options.Command;

                var (isHealthy, _) = await command
                    .ExecuteScalarAsync(cancellationToken)
                    .WithTimeoutAsync(options.Timeout, cancellationToken)
                    .ConfigureAwait(false);

                return isHealthy
                    ? HealthCheckResult.Healthy($"{name}: Healthy")
                    : HealthCheckResult.Degraded($"{name}: Degraded");
            }
        }
    }

    /// <summary>
    /// Create a new instance of <see cref="DbConnection"/> based on the given <paramref name="connectionString"/>.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    protected abstract DbConnection CreateConnection(string connectionString);
}
