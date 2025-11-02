namespace NetEvolve.HealthChecks.Oracle;

using global::Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(OracleConnection), typeof(OracleOptions), true)]
internal sealed partial class OracleHealthCheck(IOptionsMonitor<OracleOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<OracleOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM dual";
}
