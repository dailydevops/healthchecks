namespace NetEvolve.HealthChecks.SQLite;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SqliteConnection), typeof(SQLiteOptions), true)]
internal sealed partial class SQLiteHealthCheck(IOptionsMonitor<SQLiteOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<SQLiteOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
