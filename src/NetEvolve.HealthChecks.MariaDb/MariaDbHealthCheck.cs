namespace NetEvolve.HealthChecks.MariaDb;

using MySqlConnector;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(MySqlConnection), typeof(MariaDbOptions), true)]
internal sealed partial class MariaDbHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
