namespace NetEvolve.HealthChecks.SqlServer;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

internal sealed class SqlServerHealthCheck
    : ConfigurableHealthCheckBase<SqlServerHealthCheckOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public SqlServerHealthCheck(IOptionsMonitor<SqlServerHealthCheckOptions> optionsMonitor)
        : base(optionsMonitor) { }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "As designed."
    )]
    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SqlServerHealthCheckOptions options,
        CancellationToken cancellationToken
    )
    {
        using (var connection = new SqlConnection(options.ConnectionString))
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = string.IsNullOrWhiteSpace(options.Command)
                    ? DefaultCommand
                    : options.Command;

                var (isHealthy, result) = await command
                    .ExecuteScalarAsync(cancellationToken)
                    .WithTimeoutAsync(options.Timeout, cancellationToken)
                    .ConfigureAwait(false);

                return isHealthy && (int)result == 1
                    ? HealthCheckResult.Healthy($"{name}: Healthy")
                    : HealthCheckResult.Degraded($"{name}: Degraded");
            }
        }
    }
}
