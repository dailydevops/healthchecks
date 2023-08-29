namespace NetEvolve.HealthChecks.SqlServer.Legacy;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

internal sealed class SqlServerCheck : ConfigurableHealthCheckBase<SqlServerOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public SqlServerCheck(IOptionsMonitor<SqlServerOptions> optionsMonitor)
        : base(optionsMonitor) { }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "As designed."
    )]
    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SqlServerOptions options,
        CancellationToken cancellationToken
    )
    {
        using (var connection = new SqlConnection(options.ConnectionString))
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
}
