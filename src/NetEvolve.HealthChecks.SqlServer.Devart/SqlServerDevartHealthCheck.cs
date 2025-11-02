namespace NetEvolve.HealthChecks.SqlServer.Devart;

using global::Devart.Data.SqlServer;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SqlConnection), typeof(SqlServerDevartOptions), false)]
internal sealed partial class SqlServerDevartHealthCheck(IOptionsMonitor<SqlServerDevartOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<SqlServerDevartOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
