namespace NetEvolve.HealthChecks.Odbc;

using System.Data.Odbc;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(OdbcConnection), typeof(OdbcOptions), true)]
internal sealed partial class OdbcHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
