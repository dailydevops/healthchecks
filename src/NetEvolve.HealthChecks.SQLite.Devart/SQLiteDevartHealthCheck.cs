namespace NetEvolve.HealthChecks.SQLite.Devart;

using global::Devart.Data.SQLite;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SQLiteConnection), typeof(SQLiteDevartOptions), true)]
internal sealed partial class SQLiteDevartHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
