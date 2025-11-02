namespace NetEvolve.HealthChecks.MySql.Connector;

using Microsoft.Extensions.Options;
using MySqlConnector;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(MySqlConnection), typeof(MySqlOptions), true)]
internal sealed partial class MySqlHealthCheck(IOptionsMonitor<MySqlOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<MySqlOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
