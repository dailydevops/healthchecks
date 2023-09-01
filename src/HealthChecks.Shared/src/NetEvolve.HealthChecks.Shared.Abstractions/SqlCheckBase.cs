#if USE_SQL_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

internal abstract class SqlCheckBase<TConfiguration> : ConfigurableHealthCheckBase<TConfiguration>
    where TConfiguration : class, ISqlCheckOptions
{
    protected SqlCheckBase(IOptionsMonitor<TConfiguration> optionsMonitor)
        : base(optionsMonitor) { }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "As designed."
    )]
    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
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

    protected abstract DbConnection CreateConnection(string connectionString);
}
#endif
