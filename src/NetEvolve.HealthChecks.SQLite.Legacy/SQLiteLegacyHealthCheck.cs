namespace NetEvolve.HealthChecks.SQLite.Legacy;

using System.Data.SQLite;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SQLiteConnection), typeof(SQLiteLegacyOptions), true)]
internal sealed partial class SQLiteLegacyHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
