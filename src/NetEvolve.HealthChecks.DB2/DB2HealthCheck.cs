namespace NetEvolve.HealthChecks.DB2;

using IBM.Data.Db2;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(DB2Connection), typeof(DB2Options), true)]
internal sealed partial class DB2HealthCheck(IOptionsMonitor<DB2Options> optionsMonitor)
    : ConfigurableHealthCheckBase<DB2Options>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";
}
