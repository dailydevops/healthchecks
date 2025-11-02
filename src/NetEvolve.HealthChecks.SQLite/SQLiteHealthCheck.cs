namespace NetEvolve.HealthChecks.SQLite;

using Microsoft.Data.Sqlite;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SqliteConnection), typeof(SQLiteOptions), true)]
internal sealed partial class SQLiteHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
