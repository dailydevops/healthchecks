namespace NetEvolve.HealthChecks.Oracle;

using global::Oracle.ManagedDataAccess.Client;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(OracleConnection), typeof(OracleOptions), true)]
internal sealed partial class OracleHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM dual";
}
