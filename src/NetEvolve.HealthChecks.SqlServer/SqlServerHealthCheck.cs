namespace NetEvolve.HealthChecks.SqlServer;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(SqlConnection), typeof(SqlServerOptions), false)]
internal sealed partial class SqlServerHealthCheck(IOptionsMonitor<SqlServerOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<SqlServerOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
