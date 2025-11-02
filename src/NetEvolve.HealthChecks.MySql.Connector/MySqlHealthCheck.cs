namespace NetEvolve.HealthChecks.MySql.Connector;

using MySqlConnector;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(MySqlConnection), typeof(MySqlOptions), true)]
internal sealed partial class MySqlHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
