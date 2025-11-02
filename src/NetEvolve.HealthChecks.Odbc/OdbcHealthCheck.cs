namespace NetEvolve.HealthChecks.Odbc;

using System.Data.Odbc;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(OdbcConnection), typeof(OdbcOptions), true)]
internal sealed partial class OdbcHealthCheck(IOptionsMonitor<OdbcOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<OdbcOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
