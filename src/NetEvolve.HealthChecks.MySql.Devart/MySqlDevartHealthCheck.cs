namespace NetEvolve.HealthChecks.MySql.Devart;

using global::Devart.Data.MySql;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(MySqlConnection), typeof(MySqlDevartOptions), false)]
internal sealed partial class MySqlDevartHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
