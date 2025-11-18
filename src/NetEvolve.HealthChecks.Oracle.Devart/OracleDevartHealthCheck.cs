namespace NetEvolve.HealthChecks.Oracle.Devart;

using global::Devart.Data.Oracle;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(OracleConnection), typeof(OracleDevartOptions), false)]
internal sealed partial class OracleDevartHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM dual";
}
