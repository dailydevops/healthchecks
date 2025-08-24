namespace NetEvolve.HealthChecks.MySql.Devart;

using global::Devart.Data.MySql;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(MySqlConnection), typeof(MySqlDevartOptions), true)]
internal sealed partial class MySqlDevartHealthCheck(IOptionsMonitor<MySqlDevartOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<MySqlDevartOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}