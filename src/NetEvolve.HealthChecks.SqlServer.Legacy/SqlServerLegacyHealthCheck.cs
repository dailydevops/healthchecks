namespace NetEvolve.HealthChecks.SqlServer.Legacy;

using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SqlConnection), typeof(SqlServerLegacyOptions), false)]
internal sealed partial class SqlServerLegacyHealthCheck(IOptionsMonitor<SqlServerLegacyOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<SqlServerLegacyOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
