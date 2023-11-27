#if USE_SQL_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using NetEvolve.Extensions.Tasks;

internal abstract class SqlCheckBase<TConfiguration> : IHealthCheck
    where TConfiguration : class, ISqlCheckOptions
{
    private readonly IOptionsMonitor<TConfiguration> _optionsMonitor;

    protected SqlCheckBase(IOptionsMonitor<TConfiguration> optionsMonitor)
        => _optionsMonitor = optionsMonitor;
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        Argument.ThrowIfNull(context);

        var configurationName = context.Registration.Name;
        var failureStatus = context.Registration.FailureStatus;

        if (cancellationToken.IsCancellationRequested)
        {
            return new HealthCheckResult(
                failureStatus,
                description: $"{configurationName}: Cancellation requested."
                );
        }

        return await InternalAsync(configurationName, failureStatus, cancellationToken).ConfigureAwait(false);
    }


    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "As designed."
    )]
    private async Task<HealthCheckResult> InternalAsync(string name, HealthStatus failureStatus, CancellationToken cancellationToken)
    {
        try
        {
            var options = _optionsMonitor.Get(name);
            if (options is null)
            {
                return new HealthCheckResult(
                    HealthStatus.Unhealthy,
                    description: $"{name}: Missing configuration."
                );
            }


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
        catch (Exception ex)
        {
            return new HealthCheckResult(failureStatus, description: $"{name}: Unexpected error.", exception: ex);
        }
    }

    protected abstract DbConnection CreateConnection(string connectionString);
}
#endif
