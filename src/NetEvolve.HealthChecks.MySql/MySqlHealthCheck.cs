namespace NetEvolve.HealthChecks.MySql;

using global::MySql.Data.MySqlClient;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(MySqlConnection), typeof(MySqlOptions), true)]
internal sealed partial class MySqlHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
