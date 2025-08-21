namespace NetEvolve.HealthChecks.Oracle.Devart;

using global::Devart.Data.Oracle;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(OracleConnection), typeof(OracleDevartOptions), true)]
internal sealed partial class OracleDevartHealthCheck(IOptionsMonitor<OracleDevartOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<OracleDevartOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM dual";
}