namespace NetEvolve.HealthChecks.SqlServer.Legacy;

using System.Data.SqlClient;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SqlConnection), typeof(SqlServerLegacyOptions), false)]
internal sealed partial class SqlServerLegacyHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
