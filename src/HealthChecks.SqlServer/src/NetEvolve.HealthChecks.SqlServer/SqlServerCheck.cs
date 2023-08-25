namespace NetEvolve.HealthChecks.SqlServer;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;
using System.Threading;
using System.Threading.Tasks;

internal sealed class SqlServerCheck : ConfigurableHealthCheckBase<SqlServerCheckOptions>
{
    public SqlServerCheck(IOptionsMonitor<SqlServerCheckOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        SqlServerCheckOptions options,
        HealthCheckContext context,
        CancellationToken cancellationToken
    )
    {
        using (var connection = new SqlConnection(options.ConnectionString))
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT 1;";

                var (isHealthy, result) = await command
                    .ExecuteScalarAsync(cancellationToken)
                    .WithTimeoutAsync(options.Timeout, cancellationToken)
                    .ConfigureAwait(false);

                return isHealthy && (int)result == 1
                    ? HealthCheckResult.Healthy()
                    : HealthCheckResult.Degraded();
            }
        }
    }
}
