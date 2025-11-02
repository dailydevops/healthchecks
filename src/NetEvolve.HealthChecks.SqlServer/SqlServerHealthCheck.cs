namespace NetEvolve.HealthChecks.SqlServer;

using Microsoft.Data.SqlClient;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SqlConnection), typeof(SqlServerOptions), false)]
internal sealed partial class SqlServerHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
