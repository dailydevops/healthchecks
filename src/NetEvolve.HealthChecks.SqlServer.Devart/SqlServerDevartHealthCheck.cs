namespace NetEvolve.HealthChecks.SqlServer.Devart;

using global::Devart.Data.SqlServer;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SqlConnection), typeof(SqlServerDevartOptions), false)]
internal sealed partial class SqlServerDevartHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
