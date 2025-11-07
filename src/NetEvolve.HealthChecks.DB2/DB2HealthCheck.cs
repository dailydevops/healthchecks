namespace NetEvolve.HealthChecks.DB2;

using IBM.Data.Db2;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(DB2Connection), typeof(DB2Options), false)]
internal sealed partial class DB2HealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";
}
