namespace NetEvolve.HealthChecks.DB2.Devart;

using global::Devart.Data.DB2;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(DB2Connection), typeof(DB2DevartOptions), false)]
internal sealed partial class DB2DevartHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";
}
