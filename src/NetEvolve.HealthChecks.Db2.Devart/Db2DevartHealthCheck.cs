namespace NetEvolve.HealthChecks.Db2.Devart;

using global::Devart.Data.DB2;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(DB2Connection), typeof(Db2DevartOptions), true)]
internal sealed partial class Db2DevartHealthCheck(IOptionsMonitor<Db2DevartOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<Db2DevartOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";
}